﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EshopWorld.Strada.Tests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EshopWorld.Strada.Tests.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Endpoint=sb://compressionchannel.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uxLxarLV/vinlnqGW/0SwdTdqUYud/BDZVT9HKscgMM=.
        /// </summary>
        internal static string CompressionEventHubsConnectionString {
            get {
                return ResourceManager.GetString("CompressionEventHubsConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to integration-test.
        /// </summary>
        internal static string EventHubsName {
            get {
                return ResourceManager.GetString("EventHubsName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [{&quot;id&quot;:1,&quot;first_name&quot;:&quot;Regine&quot;,&quot;last_name&quot;:&quot;Tallis&quot;,&quot;email&quot;:&quot;rtallis0@ca.gov&quot;,&quot;gender&quot;:&quot;Female&quot;,&quot;ip_address&quot;:&quot;124.243.16.25&quot;},{&quot;id&quot;:2,&quot;first_name&quot;:&quot;Meryl&quot;,&quot;last_name&quot;:&quot;Tigner&quot;,&quot;email&quot;:&quot;mtigner1@soup.io&quot;,&quot;gender&quot;:&quot;Female&quot;,&quot;ip_address&quot;:&quot;3.255.212.100&quot;},{&quot;id&quot;:3,&quot;first_name&quot;:&quot;Noami&quot;,&quot;last_name&quot;:&quot;Morfell&quot;,&quot;email&quot;:&quot;nmorfell2@comcast.net&quot;,&quot;gender&quot;:&quot;Female&quot;,&quot;ip_address&quot;:&quot;108.27.148.195&quot;},{&quot;id&quot;:4,&quot;first_name&quot;:&quot;Sosanna&quot;,&quot;last_name&quot;:&quot;Langmaid&quot;,&quot;email&quot;:&quot;slangmaid3@usda.gov&quot;,&quot;gender&quot;:&quot;Female&quot;,&quot;ip_address&quot;:&quot;100.124.62.71 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string EventHubsPayload {
            get {
                return ResourceManager.GetString("EventHubsPayload", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Endpoint=sb://strada.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=atxvtCnKnvvqiQDyD0G0+TUGqEaBEkXo660tUM/uGxA=.
        /// </summary>
        internal static string ServiceBusConnectionString {
            get {
                return ResourceManager.GetString("ServiceBusConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DefaultEndpointsProtocol=https;AccountName=strada;AccountKey=/QqqyGc4m26ggNIrCcC28OXJyOcmKaXnudYVrEo9Jn6P+aHiriwu63uz1/ZfayFPtkQox6PW5EKgAIhFMZF3wA==;EndpointSuffix=core.windows.net.
        /// </summary>
        internal static string StorageConnectionString {
            get {
                return ResourceManager.GetString("StorageConnectionString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to integration-test.
        /// </summary>
        internal static string SubscriptionName {
            get {
                return ResourceManager.GetString("SubscriptionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to integration-test.
        /// </summary>
        internal static string TopicName {
            get {
                return ResourceManager.GetString("TopicName", resourceCulture);
            }
        }
    }
}
