using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Cryptography;

public class Program
{
    


    
    [STAThread]
    static void Main()
    {
        Serialize();
        Deserialize();
        Console.ReadLine();
        
    }

    static void Serialize()
    {
        // Create a hashtable of values that will eventually be serialized.
        string sifrelenmisText = "";
        string deneme = "Sanlab deneme";
        byte[] data = UTF8Encoding.UTF8.GetBytes(deneme);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(deneme));
            using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform transform = tripDes.CreateEncryptor();
                byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                 sifrelenmisText= Convert.ToBase64String(results, 0, results.Length);
            }
        }
        Console.WriteLine(sifrelenmisText);
        // To serialize the hashtable and its key/value pairs,  
        // you must first open a stream for writing. 
        // In this case, use a file stream.
        FileStream fs = new FileStream("DataFile.san", FileMode.Create);

        // Construct a BinaryFormatter and use it to serialize the data to the stream.
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(fs, sifrelenmisText);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to serialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }
    }


    static void Deserialize()
    {
        // Declare the hashtable reference.
        string sifreliData = null;
        string cozulmusText = "";

        // Open the file containing the data that you want to deserialize.
        FileStream fs = new FileStream("DataFile.san", FileMode.Open);
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            // Deserialize the hashtable from the file and 
            // assign the reference to the local variable.
            


            sifreliData = (string)formatter.Deserialize(fs);

            byte[] sifreli = Convert.FromBase64String(sifreliData);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes("Sanlab deneme"));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider() { Key = keys, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(sifreli, 0, sifreli.Length);
                    cozulmusText = UTF8Encoding.UTF8.GetString(results);
                }
            }



            Console.WriteLine(cozulmusText);
        }
        catch (SerializationException e)
        {
            Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
            throw;
        }
        finally
        {
            fs.Close();
        }

        // To prove that the table deserialized correctly, 
        // display the key/value pairs.
        //foreach (DictionaryEntry de in addresses)
        //{
        //    Console.WriteLine("{0} lives at {1}.", de.Key, de.Value);
        //}
        
       
    }
   

   
}