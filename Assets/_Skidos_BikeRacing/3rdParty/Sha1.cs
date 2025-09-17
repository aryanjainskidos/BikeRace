using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;


public class Sha1 {

	public static string Sha1Sum(string strToEncrypt)
	{
		UTF8Encoding ue = new UTF8Encoding();
		byte[] bytes = ue.GetBytes(strToEncrypt);
		
		// encrypt bytes
		SHA1 sha = new SHA1CryptoServiceProvider();
		byte[] hashBytes = sha.ComputeHash(bytes);
		
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
		
		for (int i = 0; i < hashBytes.Length; i++)
		{
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
		
		return hashString.PadLeft(32, '0');
	}
}
