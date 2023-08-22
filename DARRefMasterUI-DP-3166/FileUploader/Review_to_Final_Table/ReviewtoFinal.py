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

class cryptoReviewDatatoTable:

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
        self.review_dataframe = self.read_file_from_s3()
        self.upload_data_to_rds_table()
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
        """
        Reads the data from review bucket and returns it in the form of dataframe
        """
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
            #print(df)
        except Exception as e:
            self.message = self.message + "\n" + 'File upload to S3 procedure failed' + e
        finally:
            # self.slack_alert()
            print('File upload to RDS table')
        return df

    def upload_data_to_rds_table(self):
        """
        Reads the data from staging crypto table and returns it in the form of dataframe
        """
        sql_query = 'DELETE FROM [ReferenceCore].[dbo].[EventInformation] where convert(varchar(12),createTime,112)=CONVERT(varchar(12),DATEADD(DAY,' + str(CURRENT_DATE_MINUS) +', GETDATE()), 112)'
        self.cursor.execute(sql_query)
        df = self.review_dataframe.replace(r'^\s*$', np.nan, regex=True)
        df['EventDate'] = df['EventDate'].replace(r'^([A-Za-z])+$', np.NaN, regex=True)
        df['AnnouncementDate'] = df['AnnouncementDate'].replace(r'^([A-Za-z])+$', np.NaN, regex=True)
        df['EventDate'] = pd.to_datetime(df['EventDate'])
        df['AnnouncementDate'] = pd.to_datetime(df['AnnouncementDate'])
        df = df.replace({np.NaN: None})
        for index, row in df.iterrows():
            self.cursor.execute(
                "INSERT INTO [ReferenceCore].[dbo].[EventInformation] (DateofReview, ExchangeAssetTicker, ExchangeAssetName, "
                "DARAssetId, DAREventID, EventType, EventDate, AnnouncementDate, EventDescription, SourceURL, Notes, Deleted, Exchange) values(?,?,?,?,?,?,?,?,?,?,?,?,?)",
                row['DateofReview'], row['ExchangeAssetTicker'], row['ExchangeAssetName'], row['DARAssetID'],
                row['DAREventID'], row['EventType'], row['EventDate'], row['AnnouncementDate'],
                row['EventDescription'], row['SourceURL'], row['Notes'], row['Deleted'], row['Exchange'])

if __name__ == '__main__':
    cryptoReviewDatatoTable()