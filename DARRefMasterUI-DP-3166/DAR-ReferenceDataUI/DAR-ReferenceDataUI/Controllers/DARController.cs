using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using DARReferenceData;

namespace DAR_ReferenceDataUI.Controllers
{

    public class DARController: Controller
    {

        private DARAdmin dhAdmin = new DARAdmin();

        public string GetCurrentModuleName()
        {
            var callingMethod = new System.Diagnostics.StackTrace(2, false).GetFrame(0).GetMethod();
            return $"{callingMethod.DeclaringType.FullName}.{callingMethod.Name}";
        }

        public ActionResult RedirectToLogin()
        {
            return RedirectToAction("LogIn", "Home");
        }

        public ActionResult RedirectToInsufficientAcess(string moduleName)
        {
            return RedirectToAction("InsufficientAccess", "Home",new { message = $"Request access to Module: {moduleName}" } );
        }

        public bool UserHasAccess()
        {
            bool userHasAccess = false;

            if (!User.Identity.IsAuthenticated)
            {
                return userHasAccess;
            }
            string currentModuleName = GetCurrentModuleName();
            if(!DARApplicationInfo.DARRoles.ContainsKey(currentModuleName))
            {
                return userHasAccess;
            }


            // Load all modles and corresponding roles in a dictionary in DARReferenceData.SomeClass.
            // Lookup if the current user is in one of these roles. 
            // Then allow access

            if(!DARApplicationInfo.DARRoles.ContainsKey(currentModuleName))
            {
                return userHasAccess;
            }


            var roles = DARApplicationInfo.DARRoles[currentModuleName];
            if(roles != null || roles.Any())
            {
                foreach (var x in roles)
                {
                    if (User.IsInRole(x))
                    {
                        userHasAccess = true;
                        break;
                    }
                }
            }

            return userHasAccess;
    

        }

        public ActionResult ValidateUser()
        {
            bool userHasAccess = false;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToLogin();
            }

            string currentModuleName = GetCurrentModuleName();



            // Load all modles and corresponding roles in a dictionary in DARReferenceData.SomeClass.
            // Lookup if the current user is in one of these roles. 
            // Then allow access

            var moduleRoles = dhAdmin.GetModuleRoles(currentModuleName);
            if(moduleRoles == null || !moduleRoles.Any())
                return RedirectToInsufficientAcess(currentModuleName);


            foreach (var x in moduleRoles)
            {
                if (x == null)
                    continue;

                if (User.IsInRole(x))
                {
                    userHasAccess = true;
                    break;
                }
            }
         
          
            if(!userHasAccess)
            {
                return RedirectToInsufficientAcess(currentModuleName);
            }    
            else
            {
                return null;
            }
    
        }

        public ActionResult IsAllowedToAddUser()
        {
        
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToLogin();
            }


            if(!User.Identity.Name.Equals("darrefadmin"))
            {
                return RedirectToInsufficientAcess("Please login as [darrefadmin] to add a new user");
            }
            else
            {
                return null;
            }


        }

    }
}