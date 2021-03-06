﻿using System;

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
using System.Collections;
using System.Collections.Generic;

namespace GetUserInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (SPSite Sitecollection = new SPSite("<Site URL>"))                
                {
                    using (SPWeb site = Sitecollection.OpenWeb())
                    {

                        #region To get the User Information 
                        // To get the User Information
                        SPServiceContext serviceContext = SPServiceContext.GetContext(Sitecollection);
                        UserProfileManager profileManager = new UserProfileManager(serviceContext);
                        UserProfile currentProfile = profileManager.GetUserProfile("<username>");

                        // 1
                        ProfileValueCollectionBase profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.PreferredName);
                        if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                            Console.WriteLine("Name: " + profileValueCollection.Value);

                        profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.AboutMe);
                        if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                            Console.WriteLine("About Me: " + profileValueCollection.Value);

                        profileValueCollection = currentProfile.GetProfileValueCollection(PropertyConstants.Department);
                        if ((profileValueCollection != null) && (profileValueCollection.Value != null))
                            Console.WriteLine("Department: " + profileValueCollection.Value);

                        // 2
                        if (((ProfileValueCollectionBase)(currentProfile["PreferredName"])).Value != null)
                            Console.WriteLine("Name: " + currentProfile["PreferredName"].ToString());

                        if (((ProfileValueCollectionBase)(currentProfile["AboutMe"])).Value != null)
                            Console.WriteLine("About Me: " + currentProfile["AboutMe"].ToString());

                        if (((ProfileValueCollectionBase)(currentProfile["PreferredName"])).Value != null)
                            Console.WriteLine("Department: " + currentProfile["PreferredName"].ToString());

                        // To get all the User Profiles                        
                        IEnumerator profileCollection = profileManager.GetEnumerator();
                        while (profileCollection.MoveNext())
                        {
                            UserProfile Profile = (UserProfile)profileCollection.Current;
                            ProfileValueCollectionBase profileValueColl = Profile.GetProfileValueCollection(PropertyConstants.PreferredName);
                            if ((profileValueColl != null) && (profileValueColl.Value != null))
                                Console.WriteLine("Name: " + profileValueColl.Value);

                            profileValueColl = Profile.GetProfileValueCollection(PropertyConstants.AboutMe);
                            if ((profileValueColl != null) && (profileValueColl.Value != null))
                                Console.WriteLine("About Me: " + profileValueColl.Value);

                            profileValueColl = Profile.GetProfileValueCollection(PropertyConstants.Department);
                            if ((profileValueColl != null) && (profileValueColl.Value != null))
                                Console.WriteLine("Department: " + profileValueColl.Value);
                            Console.WriteLine("");
                        }
                        #endregion                        
                    }
                }
                Console.ReadLine();
            }
            catch(Exception ex)
            { throw ex; }
        }
    }    
}
