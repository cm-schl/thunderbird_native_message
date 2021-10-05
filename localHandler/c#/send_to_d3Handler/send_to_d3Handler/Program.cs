using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace send_to_d3Handler
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream inputStream = Console.OpenStandardInput();
            byte[] messageLengthArray = new byte[4];
            int messageLength = 0;
            //First 4 bytes of the stream on STDIO contain the length of the message, so first read these bytes into a buffer
            int readedCharacters = inputStream.Read(messageLengthArray, 0, 4);

            string filename;
            string fullPath;

            if (readedCharacters > 0)
            {
                //Convert MessageLength to an Integer and create a byte Array with that length
                messageLength = System.BitConverter.ToInt32(messageLengthArray, 0);

                byte[] jsonUtf8Bytes = new byte[messageLength];

                readedCharacters = inputStream.Read(jsonUtf8Bytes);

                //Check if the found characters in the stream have the expected length
                if (readedCharacters > 0 && messageLength == readedCharacters)
                {
                    var readOnlySpan = new ReadOnlySpan<byte>(jsonUtf8Bytes);

                    //Thunderbird sends a JSON encoded String that is included in double quotes. Deserialize converts it to a normal string
                    //(and would, if we wouldn't process a base64 String, also convert other things like escape characters)
                    string result_encoded = JsonSerializer.Deserialize<string>(readOnlySpan);

                    //Decode the base64 String - thereby we can save an unmodified, ASCII encoded .eml file
                    //(see https://stackoverflow.com/questions/64558872/encounter-encoding-problem-while-copying-an-email-from-thunderbird-via-delphi-us)
                    //We just use this for the filename because when the recieved byte array is converted to UTF-8 and the original mail wasn't UTF-8 encoded we loose special characters

                    string decodedNativeMessageString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result_encoded));

                    //get the first 100 characters from the string, remove the spaces and use the string as filename
                    filename = decodedNativeMessageString.Substring(0, 100);

                    //remove every space (used for padding) from the string
                    filename = filename.Replace(" ", "");

                    fullPath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\d3Archiv\" + filename + ".eml");
 
                    //byte array that contains the filename (first 100 bytes) AND the whole mail
                    byte[] decodedNativeMessageBytes = Convert.FromBase64String(result_encoded);

                    //Create an array for the content of the mail without the 100 bytes of the filename and copy the mail content to the new array
                    byte[] mailContent = new byte[decodedNativeMessageBytes.Length-100];
                    Array.Copy(decodedNativeMessageBytes, 100, mailContent, 0, decodedNativeMessageBytes.Length - 100);

                    //use everything after the first 100 characters as mail content and write it to the d.3 folder of the user
                    File.WriteAllBytes(fullPath, mailContent);

                    //Write back to the stream (STDOUT) if everything was ok
                    Stream outputStream = Console.OpenStandardOutput();

                    //Prepare first 4 bytes with the length of the response JSON String
                    byte[] responseLength = new byte[4];
                    //Write the length of the result string to the byte array (converting 16 to bytes) - 16 because this is the result string: {"result": "OK"}
                    System.BitConverter.TryWriteBytes(responseLength, 16);
                    //Write to STDOUT
                    outputStream.Write(responseLength, 0, responseLength.Length);

                    //Prepare the response string
                    byte[] responseContent = Encoding.UTF8.GetBytes("{\"result\": \"OK\"}");
                    //Write to STDOUT
                    outputStream.Write(responseContent, 0, responseContent.Length);

                }
            }

        }
    }
}
