using System;
using Bartec.WasteCollector.Auth;
using Bartec.WasteCollector.Service;

namespace Bartec.WasteCollector
{
    public class Api
    {
        private static Api _Instance;
        private string _Token;
        private DateTime _TokenExpires;

        private Api()
        {
            var PrimeToken = Token;
            Service = new WasteCollectorAPIWebServiceSoapClient("WasteCollectorAPIWebServiceSoap", Properties.Settings.Default.WasteCollectorServiceUri);
        }

        public static Api Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Api();
                }
                return _Instance;
            }
        }

        public string Token
        {
            get
            {
                // A Bartec token lasts for 60 minutes
                if (String.IsNullOrWhiteSpace(_Token) || _TokenExpires <= DateTime.Now)
                {
                    var AuthService = new AuthenticationAPIWebServiceSoapClient("AuthenticationAPIWebServiceSoap", Properties.Settings.Default.AuthenticationUri);
                    var Auth = AuthService.Authenticate(Properties.Settings.Default.ServiceUsername, Properties.Settings.Default.ServicePassword);

                    if (Auth.Errors.Error != null)
                    {
                        throw new Exception("Unable to authenticate with WasteCollector" + ((String.IsNullOrWhiteSpace(Auth.Errors.Error.Message)) ? "." : " - " + Auth.Errors.Error.Message));
                    }
                    else
                    {
                        var AuthToken = Auth.Token;
                        _TokenExpires = DateTime.Now.AddMinutes(59);
                        _Token = AuthToken.TokenString;
                    }
                }

                return _Token;
            }
        }

        public WasteCollectorAPIWebServiceSoapClient Service;
    }
}
