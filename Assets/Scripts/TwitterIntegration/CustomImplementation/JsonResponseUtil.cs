using System;
using System.Text;

public class JsonResponseUtil {

	public static string FormDataToJson(string formData) {
		/// If the data is empty, just return and empty JSON object string
		if(String.IsNullOrEmpty(formData)){
			return "{}";
		}

		/// If we already have JSON do nothing
		if((formData.IndexOf("{") == 0 && formData.LastIndexOf("}") == formData.Length - 1) ||
			 (formData.IndexOf("[") == 0 && formData.LastIndexOf("]") == formData.Length - 1)) {
				 return formData;
			 }

		/// Otherwise, we split the formdata values and make a JSON object from
		///  the key value pairs
		string[] keyValuePairs = formData.Split('&');
		StringBuilder json = new StringBuilder("{");
		int index = 1;
		int length = keyValuePairs.Length;
		foreach(string kvp in keyValuePairs){
			string[] pair = kvp.Split('=');
			json.Append("\"");
			json.Append(pair[0]);
			json.Append("\":");
			/// Test for booleans
			string rawValue = pair[1].ToLower();
			if(rawValue == "true" || rawValue == "false"){
				json.Append(rawValue);
			}else{
				json.Append("\"");
				json.Append(pair[1]);
				json.Append("\"");
			}

			if(++index <= length){
				json.Append(",");
			}
		}
		json.Append("}");

		return json.ToString();
	}

  public static string ArrayToObject (string arrayString) {
		if (arrayString.StartsWith ("[")) {
			arrayString = "{\"items\":" + arrayString + "}";
			return arrayString;
		} else {
			return arrayString;
		}
	}
}