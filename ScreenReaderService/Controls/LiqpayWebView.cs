using System;

using Android.Util;
using Android.Webkit;
using Android.Content;

using Autofac;
using RestSharp;
using Newtonsoft.Json;

using ScreenReaderService.Util;
using ScreenReaderService.Services;
using ScreenReaderService.Data;

namespace ScreenReaderService.Controls
{
    public class LiqpayWebView : WebView
    {
        private const string GET_PAYMENT_FORM_ENDPOINT = "Pay/GetPaymentForm";
        private const string LOGIN_PARAMETER_NAME = "login";

        public event EventHandler<EventArgs> OnFinished;
        private BotService BotService = DependencyHolder.Dependencies.Resolve<BotService>();

        private async void InitPreload()
        {
            string preloadHTML = "<html>" +
                                    "<style>" +
                                        "#loader {" +
                                            "border: 1.5vh solid #67A93D;" +
                                            "border-left : 1.5vh solid #FFFFFF;" +
                                            "border-radius: 50%;" +
                                            "margin-top : 45vh;" + 
                                            "width: 10vh;" +
                                            "height: 10vh;" +
                                            "animation: spin 2s linear infinite;" +
                                        "}" +
                                        "@keyframes spin {" +
                                            "0% { transform: rotate(0deg); }" +
                                            "100% { transform: rotate(360deg); }" +
                                        "}" +
                                    "</style>" +
                                    "<body><center><div id='loader'></div></center></body>" +
                                "</html>";

            LoadData(preloadHTML, "text/html", "UTF-8");

            RestClient client = new RestClient(Constants.BASE_SERVER_ADDRESS + GET_PAYMENT_FORM_ENDPOINT);
            RestRequest request = new RestRequest(Method.GET);

            request.AddParameter(LOGIN_PARAMETER_NAME, BotService.CredentialsService.Credentials.Login);

            IRestResponse response = await client.ExecuteAsync(request);

            SignedPaymentInfo paymentInfo = JsonConvert.DeserializeObject<SignedPaymentInfo>(response.Content);
            LoadForm(paymentInfo);
        }

        private void LoadForm(SignedPaymentInfo paymentInfo)
        {
            string html = "<html>" +
                            "<style> #form { margin-top : 40vh; } </style>" +
                             "<body><center>" +
                                "<form id='form' method='POST' action='https://www.liqpay.ua/api/3/checkout' accept-charset='utf-8'>" +
                                    $"<input type='hidden' id='data' name='data' value='{paymentInfo.Data}'/>" +
                                    $"<input type='hidden' id='sign' name='signature' value='{paymentInfo.Signature}'/>" +
                                    "<input type='image' src='http://static.liqpay.ua/buttons/p1ru.radius.png'/>" +
                                "</form>" +
                             "</center></body>" +
                          "</html>";

            LoadData(html, "text/html", "UTF-8");
        }

        public LiqpayWebView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InitPreload();
        }

        public LiqpayWebView(Context context) : base(context) 
        {
            InitPreload();
        }
    }
}