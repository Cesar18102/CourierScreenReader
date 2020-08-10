using System;
using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json;

using Server.API.LiqPay.Dto;
using Server.API.LiqPay.Models;

namespace Server.API.LiqPay
{
    public class LiqPay
    {
        private static readonly HashAlgorithm Sha1 = SHA1.Create();

        private const string PUBLIC_KEY = "sandbox_i74310151520";
        private const string PRIVATE_KEY = "sandbox_kgwzzF9TsmUOIJmQQeQyM4G4yrxfGJxVq64k8hLn";

        private const int VERSION = 3;
        private const string ACTION = "pay";
        private const string CURRENCY = "UAH";

        private const string SUCCESS_STATUS_VALUE = "success";

        public SignedPaymentInfo CreatePayment(PaymentPrepareModel paymentPrepare)
        {
            LiqPayPaymentInfo liqPayPayment = new LiqPayPaymentInfo(paymentPrepare)
            {
                Version = VERSION,
                Action = ACTION,
                Currency = CURRENCY,
                PublicKey = PUBLIC_KEY,
                PrivateKey = PRIVATE_KEY,
            };

            string json = JsonConvert.SerializeObject(liqPayPayment);
            byte[] dataBytes = Encoding.UTF8.GetBytes(json);
            string data = Convert.ToBase64String(dataBytes);
            string signature = GetSignature(data);

            return new SignedPaymentInfo(data, signature);
        }

        private string GetSignature(string data)
        {
            string sign = PRIVATE_KEY + data + PRIVATE_KEY;
            byte[] signBytes = Encoding.UTF8.GetBytes(sign);
            byte[] hash = Sha1.ComputeHash(signBytes);
            return Convert.ToBase64String(hash);
        }

        public bool IsPaymentAuthorized(SignedPaymentInfo payment, SignedPaymentInfo stored)
        {
            string calculatedSignature = GetSignature(payment.Data);
            string storedSignature = GetSignature(payment.Data);

            string data = Encoding.UTF8.GetString(Convert.FromBase64String(stored.Data));
            LiqPayPaymentInfo info = JsonConvert.DeserializeObject<LiqPayPaymentInfo>(data);
            //DateTime exp = DateTime.Parse(info.Expired);

            return calculatedSignature == payment.Signature && calculatedSignature == storedSignature; // && exp > DateTime.UtcNow;
        }

        public string GetOrderId(SignedPaymentInfo payment)
        {
            byte[] jsonBytes = Convert.FromBase64String(payment.Data);
            string json = Encoding.UTF8.GetString(jsonBytes);

            LiqPayPaymentInfo liqPayPayment = JsonConvert.DeserializeObject<LiqPayPaymentInfo>(json);
            return liqPayPayment.OrderId;
        }

        public bool IsSucceed(SignedPaymentInfo payment)
        {
            byte[] dataBytes = Convert.FromBase64String(payment.Data);
            string data = Encoding.UTF8.GetString(dataBytes);
            PaymentStatusDto result = JsonConvert.DeserializeObject<PaymentStatusDto>(data);
            return result.Status == SUCCESS_STATUS_VALUE;
        }
    }
}