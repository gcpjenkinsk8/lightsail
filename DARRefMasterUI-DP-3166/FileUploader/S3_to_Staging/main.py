import os
import sys
import time
import pyodbc
import boto3
import pytz
import pandas as pd
#import requests
from io import StringIO
from datetime import datetime, timedelta
from dotenv import load_dotenv
#from config import config
import numpy as np
import io
from xlsxwriter import Workbook
import openpyxl
# Loading env file
load_dotenv('configuration.env')

BUCKET = os.environ["BUCKET"]
DEST_BUCKET = os.environ["DEST_BUCKET"]
DAR_AWS_KEY = os.environ["DAR_AWS_KEY"]
DAR_AWS_SECRET_KEY = os.environ["DAR_AWS_SECRET_KEY"]
SERVER = os.environ["TEST_SERVER"]
USER_ID = os.environ["USER_ID"]
PASSWORD = os.environ["TEST_PASSWORD"]
PORT = os.environ["PORT"]
DATABASE = os.environ["DATABASE"]
START_HOUR = int(os.environ['START_HOUR'])
START_MINUTE = int(os.environ['START_MINUTE'])
SLACK_ALERT_WEBHOOK = str(os.environ['SLACK_ALERT_WEBHOOK'])
FILENAME = os.environ['FILENAME']
FOLDER_NAME = os.environ['FOLDER_NAME']
CURRENT_DATE_MINUS = int(os.environ['CURRENT_DATE_MINUS'])
REVIEW_FOLDER_NAME = os.environ["REVIEW_FOLDER_NAME"]

class cryptoIngestion:

    def __init__(self):
        self.message = "Crypto Node Events Report\n"
        date_today = datetime.today().astimezone(pytz.timezone('US/Eastern'))
        file_date = f'{date_today.year}{date_today.month:02d}{date_today.day:02d}'
        self.folder_name = FOLDER_NAME
        print(self.folder_name)
        # Setting up S3 Connection
        self.s3 = self.s3_connection()
        # Setting up RDS Connection
        self.cursor = self.connection_rds()
        self.read_file_from_s3()
        # self.read_reviewed_file_from_s3()
        self.cursor.close()

    def s3_connection(self):
        """
        Establish connection with aws and return S3 object.
        """
        try:
            aws_session = boto3.Session(
                aws_access_key_id=DAR_AWS_KEY,
                aws_secret_access_key=DAR_AWS_SECRET_KEY)
            #aws_s3 = aws_session.client('s3')
            aws_s3 = aws_session.resource('s3')
        except Exception as e:
            self.message = self.message + '\n' + 'S3 Connection Failed' + e
        # self.slack_alert()

        return aws_s3

    def connection_rds(self):
        """
        Establish connection with RDS and return cursor object
        """
        try:
            conn_string = 'DRIVER={};PORT=1433;SERVER={};UID={};PWD={};'.format("{ODBC Driver 17 for SQL Server}",
                                                                                SERVER,
                                                                                USER_ID, PASSWORD)
            conn = pyodbc.connect(conn_string, autocommit=True)
            cursor = conn.cursor()
        except Exception as e:
            self.message = self.message + '\n' + 'Connection to rds Failed' + e
            print(self.message)
            # self.slack_alert()

        return cursor

    def read_file_from_s3(self):
        try:
            file_list = []
            prefix_df = []
            pd.set_option('display.max_columns', 40)
            my_bucket = self.s3.Bucket('cryptoevents')
            for object_summary in my_bucket.objects.filter(Prefix=self.folder_name):
                if object_summary.key.endswith('.xlsx') or object_summary.key.endswith('.xls'):
                    file_list.append(object_summary.key)
                    print("File list", file_list)
                    key = object_summary.key
                    body = object_summary.get()['Body'].read()
                    temp = pd.read_excel(io.BytesIO(body), engine='openpyxl')
                    prefix_df.append(temp)

            df = pd.concat(prefix_df)
            df = df.replace(r'^\s*$', np.nan, regex=True)
            df['Event Date'] = df['Event Date'].replace(r'^([A-Za-z])+$', np.NaN, regex=True)
            df['Announcement Date'] = df['Announcement Date'].replace(r'^([A-Za-z])+$', np.NaN, regex=True)
            df['Event Date'] = pd.to_datetime(df['Event Date'])
            df['Announcement Date'] = pd.to_datetime(df['Announcement Date'])
            df = df.replace({np.NaN: None})
            #print(df)
            #print(df.columns)
            sql_query = 'DELETE FROM [ReferenceCore].[dbo].[Staging_CryptoNodeEvents] where convert(varchar(12),createTime,112)<=CONVERT(varchar(12),DATEADD(DAY,' + str(CURRENT_DATE_MINUS) + ', GETDATE()), 112)'
            self.cursor.execute(sql_query)
            if 'Exchange' in df.columns:
                print("Exchange present:\n")
                for index, row in df.iterrows():
                    self.cursor.execute("INSERT INTO [ReferenceCore].[dbo].[Staging_CryptoNodeEvents] (DateofReview, ExchangeAssetTicker, ExchangeAssetName, DARAssetId, DAREventID, EventType, EventDate, AnnouncementDate, EventDescription, SourceURL, Notes, Deleted, Exchange) values(?,?,?,?,?,?,?,?,?,?,?,?,?)",
                                        row['Date of Review'], row['Asset Ticker'], row['Asset Name'], row['DAR Asset ID'], row['DAR Event ID'], row['Event Type'], row['Event Date'], row['Announcement Date'], row['Event Description'], row['Source URL'], row['Notes'], row['Deleted'], row['Exchange'])
            else:
                print("Exchange not present:\n")
                for index, row in df.iterrows():
                    self.cursor.execute("INSERT INTO [ReferenceCore].[dbo].[Staging_CryptoNodeEvents] (DateofReview, ExchangeAssetTicker, ExchangeAssetName, DARAssetId, DAREventID, EventType, EventDate, AnnouncementDate, EventDescription, SourceURL, Notes, Deleted) values(?,?,?,?,?,?,?,?,?,?,?,?)",
                                        row['Date of Review'], row['Asset Ticker'], row['Asset Name'], row['DAR Asset ID'], row['DAR Event ID'], row['Event Type'], row['Event Date'], row['Announcement Date'], row['Event Description'], row['Source URL'], row['Notes'], row['Deleted'])
            stored_procedure = 'EXEC ReferenceCore.dbo.spCryptoNodeEvents_Listing @CurrentDateMinus = ? ; '
            param_validation = (CURRENT_DATE_MINUS)
            self.cursor.execute(stored_procedure, param_validation)
            self.cursor.commit()
            self.upload_data_to_review()
            for obj in my_bucket.objects.filter(Prefix=self.folder_name):
                self.s3.Object(DEST_BUCKET, obj.key).put(Body=obj.get()["Body"].read())
                if obj.key != (self.folder_name+'/'):
                    obj.delete()
            print('File upload to RDS table successfully completed.')
        except Exception as e:
            self.message = self.message + "\n" + 'File upload to S3 procedure failed' + e
        finally:
            # self.slack_alert()
            print('File upload to RDS table')

    def upload_data_to_review(self):
        current_date_time = datetime.now()
        current_date_time = current_date_time.strftime("%Y-%m-%d  %H:%M:%S")
        print(current_date_time)
        my_bucket = self.s3.Bucket('cryptoevents')
        try:
            csv_buffer = StringIO()
            sql_query = 'SELECT * FROM [ReferenceCore].[dbo].[Staging_CryptoNodeEvents] where convert(varchar(12),createTime,112)=CONVERT(varchar(12),DATEADD(DAY,' + str(CURRENT_DATE_MINUS) +', GETDATE()), 112)'
            print(sql_query)
            query_results = self.cursor.execute(sql_query).fetchall()
            staging_df = pd.DataFrame((tuple(t) for t in query_results),
                                  columns=['ID', 'DateofReview', 'ExchangeAssetTicker', 'ExchangeAssetName',
                                  'DARAssetID', 'DAREventID', 'EventType', 'EventDate', 'AnnouncementDate',
                                  'EventDescription', 'SourceURL', 'EventStatus', 'Notes', 'Deleted',
                                  'Exchange', 'ValidationTime', 'AssetID', 'ExchangeID', 'EventTypeID',
                                  'CreateTime', 'BlockHeight'])
            staging_df = staging_df.iloc[:, 1:15]
            staging_df['Status'] = 'UserReview'
            print(staging_df)
            with io.BytesIO() as output:
                with pd.ExcelWriter(output, engine='xlsxwriter') as writer:
                    staging_df.to_excel(writer, index=False)
                data = output.getvalue()
            self.s3.Object(BUCKET, f'UserReview/ReviewData{current_date_time}.xlsx').put(Body=data)
            for obj in my_bucket.objects.filter(Prefix=REVIEW_FOLDER_NAME):
                self.s3.Object(DEST_BUCKET, obj.key).put(Body=obj.get()["Body"].read())
            self.message = self.message + '\n' + 'File upload to S3 procedure completed successfully.'
        except Exception as e:
            self.message = self.message + "\n" + 'File upload to S3 procedure failed' + e
        finally:
            #self.slack_alert()
            print('File upload to S3 procedure worked successfully')


if __name__ == '__main__':
    cryptoIngestion()