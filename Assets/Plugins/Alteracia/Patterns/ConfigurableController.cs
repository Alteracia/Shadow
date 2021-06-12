using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Alteracia.Patterns
{
   public interface IConfigReader
   {
      string GetRelativeUrlToFile(string absolutUrl);
      void ReadConfigFile(string relativeUrl, object configurable, Action onComplete);
   }

   [Serializable]
   public class ConfigurationReader : ScriptableObject, IConfigReader
   {
      public virtual string GetRelativeUrlToFile(string absolutUrl)
      {
         throw new NotImplementedException();
      }

      public virtual void ReadConfigFile(string relativeUrl, object configurable, Action onComplete)
      {
         throw new NotImplementedException();
      }
   }

   public abstract class ConfigurableController<T> : Controller<T> where T : Controller<T>
   {
      [SerializeField]
      protected ConfigurationReader reader;
      [SerializeField]
      protected string configRelativeUrl;
      
      public void ReadConfigurationFromFile(Action callback)
      {
         if (reader == null)
         {
            Debug.LogError("No configuration reader");
            return;
         }
         reader.ReadConfigFile(configRelativeUrl, this, callback);
      }

      public string ReadConfigurationFromJson()
      {
         
#if UNITY_EDITOR
         
         var path = UnityEditor.EditorUtility.OpenFilePanel(
            "Open configuration",
            "",
            "json");
         
         if (string.IsNullOrEmpty(path)) return null;

         StreamReader freader = new StreamReader(path);
         string json = freader.ReadToEnd();
         
         JsonUtility.FromJsonOverwrite(json, this as T);
         
         if (reader != null)
            configRelativeUrl = reader.GetRelativeUrlToFile(path);
         else
         {
            Debug.LogError("No configuration reader");
            return null;
         }
         return configRelativeUrl;
            
#endif
         
         return null;
      }

      public string SaveConfigurationToJson()
      {
         
#if UNITY_EDITOR

         var path = UnityEditor.EditorUtility.SaveFilePanel(
            "Save configuration as json",
            "",
            typeof(T) + ".json",
            "json");

         if (string.IsNullOrEmpty(path)) return null;
         
         var json = JsonUtility.ToJson(this);
         
         // Get only parameters, no reference to object
         Regex regex = new Regex(@"\s*""([^""]*?)""\s*:\s*\{([^\{\}]*?)\}(,|\s|)");
         json = regex.Replace(json, "");
         // Do not save path
         regex = new Regex(@"\s*""(configRelativeUrl)"" *: *(""(.*?)""(,|\s|)|\s*\{(.*?)\}(,|\s|))");
         json = regex.Replace(json, "");
         // Clear "," in the end
         regex = new Regex(@",\s*\}");
         json = regex.Replace(json, "}");
   
         if (string.IsNullOrEmpty(json)) return null;

         StreamWriter writer = new StreamWriter(path, false);
         writer.WriteLine(json);
         writer.Close();
         
         if (reader != null)
            configRelativeUrl = reader.GetRelativeUrlToFile(path);
         else
         {
            Debug.LogError($"Can't find ConfigurationReader for {typeof(T)} Component of {this.gameObject.name}");
            return null;
         }

         return configRelativeUrl;
         
#endif
         return null;
      }
   }
}
