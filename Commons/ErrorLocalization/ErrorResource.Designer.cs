﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Golbaus_BE.Commons.ErrorLocalization {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ErrorResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Golbaus_BE.Commons.ErrorLocalization.ErrorResource", typeof(ErrorResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} already exists.
        /// </summary>
        public static string AlreadyExists {
            get {
                return ResourceManager.GetString("AlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Do not have permission to perform this action.
        /// </summary>
        public static string DoNotHavePermission {
            get {
                return ResourceManager.GetString("DoNotHavePermission", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email has not been confirmed.
        /// </summary>
        public static string EmailNotConfirm {
            get {
                return ResourceManager.GetString("EmailNotConfirm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email verification successful.
        /// </summary>
        public static string EmailVerificationSuccessful {
            get {
                return ResourceManager.GetString("EmailVerificationSuccessful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incorrect {0}.
        /// </summary>
        public static string Incorrect {
            get {
                return ResourceManager.GetString("Incorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} invalid..
        /// </summary>
        public static string Invalid {
            get {
                return ResourceManager.GetString("Invalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} must have at least {1} characters..
        /// </summary>
        public static string LengthRequired {
            get {
                return ResourceManager.GetString("LengthRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Username or password is incorrect.
        /// </summary>
        public static string LoginFail {
            get {
                return ResourceManager.GetString("LoginFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} is required.
        /// </summary>
        public static string MissingRequired {
            get {
                return ResourceManager.GetString("MissingRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} not found..
        /// </summary>
        public static string NotFound {
            get {
                return ResourceManager.GetString("NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The password and confirm password do not match.
        /// </summary>
        public static string PasswordNotMatch {
            get {
                return ResourceManager.GetString("PasswordNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The publication time must be at least 30 minutes after the current time.
        /// </summary>
        public static string PublishTime {
            get {
                return ResourceManager.GetString("PublishTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Activation link has expired.
        /// </summary>
        public static string TokenExpried {
            get {
                return ResourceManager.GetString("TokenExpried", resourceCulture);
            }
        }
    }
}
