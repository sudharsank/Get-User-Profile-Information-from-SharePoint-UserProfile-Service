using System;

using Microsoft.SharePoint;
using Microsoft.Office.Server.UserProfiles;

using System.Runtime.Serialization.Json;
using System.Net;
using System.IO;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Web;

namespace GetUserInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (SPSite Sitecollection = new SPSite("http://172.16.12.54:1111"))
                {
                    using (SPWeb site = Sitecollection.OpenWeb())
                    {

                        #region To get the User Information 
                        //// To get the User Information
                        //SPServiceContext serviceContext = SPServiceContext.GetContext(Sitecollection);
                        //UserProfileManager profileManager = new UserProfileManager(serviceContext);
                        //UserProfile currentProfile = profileManager.GetUserProfile("<username>");

                        //// 1
                        //ProfileValueCollectionBase profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.PreferredName);
                        //if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                        //    Console.WriteLine("Name: " + profileValueCollection.Value);

                        //profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.AboutMe);
                        //if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                        //    Console.WriteLine("About Me: " + profileValueCollection.Value);

                        //profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.Department);
                        //if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                        //    Console.WriteLine("Department: " + profileValueCollection.Value);

                        //// 2
                        //if (((ProfileValueCollectionBase)(currentProfile["PreferredName"])).Value != null)
                        //    Console.WriteLine("Name: " + currentProfile["PreferredName"].ToString());

                        //if (((ProfileValueCollectionBase)(currentProfile["AboutMe"])).Value != null)
                        //    Console.WriteLine("About Me: " + currentProfile["AboutMe"].ToString());

                        //if (((ProfileValueCollectionBase)(currentProfile["PreferredName"])).Value != null)
                        //    Console.WriteLine("Department: " + currentProfile["PreferredName"].ToString());
                        #endregion

                        AdmAccessToken admToken;
                        string headerValue;
                        //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
                        AdmAuthentication admAuth = new AdmAuthentication("GetUserInfo", "LP6fdpK1OEQi17gQZHAXKxfssYRU+7F3KcAhFAVflQM=");
                        try
                        {
                            admToken = admAuth.GetAccessToken();

                            // Create a header with the access_token property of the returned token
                            headerValue = "Bearer " + admToken.access_token;
                            //headerValue = "Bearer http%3a%2f%2fschemas.xmlsoap.org%2fws%2f2005%2f05%2fidentity%2fclaims%2fnameidentifier=GetUserInfo&http%3a%2f%2fschemas.microsoft.com%2faccesscontrolservice%2f2010%2f07%2fclaims%2fidentityprovider=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&Audience=http%3a%2f%2fapi.microsofttranslator.com&ExpiresOn=1347269573&Issuer=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&HMACSHA256=jKYl156pF%2fzWMiSwC0gaLhTDjWbwQAC%2b4aONbS41swU%3d";
                            TranslateMethod(headerValue);
                        }
                        catch (WebException e)
                        {
                            //ProcessWebException(e);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                        }
                    }
                }
                Console.ReadLine();
            }
            catch(Exception ex)
            { throw ex; }
        }

        private static void TranslateMethod(string authToken)
        {

            string text = "Use pixels to express measurements for padding and margins.";
            string from = "en";
            string to = "ar";

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                    string translation = (string)dcs.ReadObject(stream);
                    Console.WriteLine("Translation for source text '{0}' from {1} to {2} is", text, "en", "de");
                    Console.WriteLine(translation);

                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }
        private static void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            string strResponse = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }

    }

    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        //public static readonly string DatamarketAccessUri = "https://datamarket.azure.com/developer/applications/";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
