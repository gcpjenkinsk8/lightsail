import io
import sys
import time
import requests
import json
import pandas as pd
import os
import boto3
from dotenv import load_dotenv
import datetime
import warnings

load_dotenv('config.env')

BUCKET = str(os.environ['BUCKET'])
DAR_AWS_KEY = os.environ['DAR_AWS_KEY']
DAR_AWS_SECRET_KEY = os.environ['DAR_AWS_SECRET_KEY']

def s3_connection():
    """
    Establish connection with aws and return S3 object.
    """
    try:
        aws_session = boto3.Session(
            aws_access_key_id=DAR_AWS_KEY,
            aws_secret_access_key=DAR_AWS_SECRET_KEY)
        # aws_s3 = aws_session.client('s3')
        aws_s3 = aws_session.resource('s3')
    except Exception as e:
        print(str(e))

    return aws_s3

def data_extraction(argv):
    s3 = s3_connection()
    dirs = os.listdir()
    # print(dirs)
    if ('data' not in dirs):
        os.makedirs('data')

    start_block = int(argv[1])
    end_block = int(argv[2])
    #end_block = 86069872
    #start_block = 86065873

    for i in range(end_block, start_block, -1):
        try:
            # url = "https://public-node-api.klaytnapi.com/v1/cypress"
            url = "http://klaytn.digitalassetresearch.com:8551/"

            payload = json.dumps({"jsonrpc": "2.0", "method": "klay_getBlockByNumber",
                                  "params": [i, True], "id": 1})
            headers = {
                'Content-Type': 'application/json'
            }
            response = requests.request("POST", url, headers=headers, data=payload)

            response = response.json()
            response_df = pd.json_normalize(response['result']['transactions'])
            timestamp = response['result']['timestamp']
            response_df['block_signed_at'] = int(timestamp, 16)
            response_df['block_signed_at'] = pd.to_datetime(response_df['block_signed_at'], unit='s')
            print(f"Block: {i}, Time: {datetime.datetime.now()}")
            get_transaction_data(response_df, i, s3)
        except:
            with open('missingBlocks_Klaytn5.txt', 'a') as file:
                file_str = str(i) + '\n'
                file.write(file_str)
            file.close()

def get_transaction_data(transaction_df, block_number,s3_conn):
    final_df = pd.DataFrame()
    for index, row in transaction_df.iterrows():
        tx_hash = row['hash']
        timestamp = row['block_signed_at']
        tx_df = getTxhash(tx_hash, timestamp)
        warnings.filterwarnings('ignore')
        final_df = final_df.append(tx_df)

    if len(final_df) > 0:
        csv_buffer = io.StringIO()
        final_df.to_csv(csv_buffer)
        s3_conn.Object(BUCKET, f'Klaytn5/{block_number}.csv').put(Body=csv_buffer.getvalue())
        print(f"{block_number} uploaded to S3")
        with open('completed_blocks_Klaytn5.txt', 'a') as file:
            file_str = str(block_number) + '\n'
            file.write(file_str)
        file.close()
    else:
        with open('blocks_without_wemix_Klaytn5.txt', 'a') as file1:
            file_str = str(block_number) + '\n'
            file1.write(file_str)
        file1.close()

def getTxhash(tx_hash, timestamp):
    from_li = []
    to_li = []
    amount_li = []
    blockHash_li = []
    blockNumber_li = []
    gas_li = []
    gasPrice_li = []
    gasUsed_li = []
    tx_hash_li = []
    type_li = []
    timestamp_li = []
    try:
        url = "http://klaytn.digitalassetresearch.com:8551/"
        # url = "https://public-node-api.klaytnapi.com/v1/cypress"

        payload = json.dumps({"jsonrpc": "2.0", "method": "klay_getTransactionReceipt",
                              "params": [str(tx_hash)], "id": 1})
        headers = {
            'Content-Type': 'application/json'
        }
        response = requests.request("POST", url, headers=headers, data=payload)

        if response.status_code != 200:
            print('sleeping for 5sec due to retries')
            time.sleep(5)
            response = requests.request("POST", url, headers=headers, data=payload)

        response = response.json()
        response = pd.json_normalize(response['result'])

        log_response = response['logs']

        for log in log_response:
            log_len = len(log)
            for i in range(0, log_len):
                # print(log[i])
                if log[i]['address'] == '0x5096db80b21ef45230c9e423c373f1fc9c0198dd' and len(log[i]['data']) == 66 and \
                        log[i]['topics'][0] == '0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef':
                    tx_hash_li.append(tx_hash)
                    blockHash_li.append(response['blockHash'][0])
                    blockNumber_li.append(int(response['blockNumber'][0], 16))
                    from_val = '0x' + log[i]['topics'][1][26:]
                    from_li.append(from_val)
                    to_val = '0x' + log[i]['topics'][2][26:]
                    to_li.append(to_val)
                    amount = int(log[i]['data'], 16)
                    amount = amount / (10 ** 18)
                    amount_li.append(amount)
                    gas_li.append(int(response['gas'][0], 16))
                    gasUsed_li.append(int(response['gasUsed'][0], 16))
                    gasPrice_li.append(int(response['gasPrice'][0], 16))
                    type_li.append(response['type'][0])
                    timestamp_li.append(timestamp)

        df = pd.DataFrame({
                'tx_hash': tx_hash_li,
                'block_height': blockNumber_li,
                'from': from_li,
                'to': to_li,
                'amount': amount_li,
                'gas_offered': gas_li,
                'gas_spent': gasUsed_li,
                'gas_price': gasPrice_li,
                'Type': type_li,
                'block_signed_at': timestamp_li
            })

        if len(df) > 0:
            df['token_name'] = 'WEMIX TOKEN'
            df['token_address'] = '0x5096db80b21ef45230c9e423c373f1fc9c0198dd'
            df = df[['tx_hash', 'block_signed_at', 'block_height', 'token_name',
                 'token_address', 'Type', 'gas_offered', 'gas_spent',
                 'gas_price', 'from', 'to', 'amount']]

            df.set_index('tx_hash', inplace=True)
            return df
        else:
            with open('transactions_without_wemix_Klaytn5.txt', 'a') as file:
                file_str = str(tx_hash) + '\n'
                file.write(file_str)
            file.close()

    except:
        with open('missingTransactions_Klaytn5.txt', 'a') as file:
            file_str = str(tx_hash) + '\n'
            file.write(file_str)
        file.close()

if __name__ == "__main__":
    data_extraction(sys.argv)
