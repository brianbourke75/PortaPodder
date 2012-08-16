using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GPodder.PortaPodder {

  /// <summary>
  /// this class will generate encrypted stuff into the android sandbox
  /// </summary>
  public class EncryptedPreferences : Java.Lang.Object, ISharedPreferences {

    #region members

    /// <summary>
    /// The key for the username
    /// </summary>
    public const string KEY_USERNAME = "username";

    /// <summary>
    /// The key for the password
    /// </summary>
    public const string KEY_PASSWORD = "password";

    /// <summary>
    /// Say that 10 times real fast!
    /// </summary>
    private const string PREFERENCES_NAME = "EncryptedPortaPodderPreferences";

    /// <summary>
    /// The unpublicized stuff... shhhh!
    /// </summary>
    private const string SEKRIT = "p00pd0ggy";

    /// <summary>
    /// The opposite of the peppa for the encryption
    /// </summary>
    private static byte[] yummy = Encoding.ASCII.GetBytes("o6806642kbM7c5");

    /// <summary>
    /// The child shared preferences to encrypt data to
    /// </summary>
    protected ISharedPreferences child = null;

    #endregion

    #region construction

    /// <summary>
    /// Initializes a new instance of the <see cref="PortaPodder.ObscuredSharedPreferences"/> class.
    /// </summary>
    /// <param name='context'>Context.</param>
    /// <param name='child'>Child.</param>
    public EncryptedPreferences() {
      child = Application.Context.GetSharedPreferences(PREFERENCES_NAME, FileCreationMode.Private);
    }

    #endregion

    #region methods

    /// <summary>
    /// Edit this instance.
    /// </summary>
    public ISharedPreferencesEditor Edit() {
      return new EncryptedEditor(this);
    }

    /// <summary>
    /// Retrieve a boolean value from the preferences.
    /// </summary>
    /// <param name='key'>The name of the preference to retrieve.</param>
    /// <param name='defValue'> Value to return if this preference does not exist.</param>
    /// <returns>The boolean.</returns>
    public bool GetBoolean(string key, bool defValue) {
      string v = child.GetString(key, null);
      return v != null ? bool.Parse(Decrypt(v)) : defValue;
    }

    /// <summary>
    /// Retrieve a float value from the preferences.
    /// </summary>
    /// <param name='key'>The name of the preference to retrieve.</param>
    /// <param name='defValue'>Value to return if this preference does not exist.</param>
    /// <returns>The float.</returns>
    public float GetFloat(string key, float defValue) {
      string v = child.GetString(key, null);
      return v != null ? float.Parse(Decrypt(v)) : defValue;
    }

    /// <summary>
    /// Retrieve a int value from the preferences.
    /// </summary>
    /// <param name='key'>The name of the preference to retrieve.</param>
    /// <param name='defValue'>Value to return if this preference does not exist.</param>
    /// <returns>The int.</returns>
    public int GetInt(string key, int defValue) {
      string v = child.GetString(key, null);
      return v != null ? int.Parse(Decrypt(v)) : defValue;
    }

    /// <summary>
    /// Retrieve a long value from the preferences.
    /// </summary>
    /// <param name='key'>The name of the preference to retrieve.</param>
    /// <param name='defValue'>Value to return if this preference does not exist.</param>
    /// <returns>The long.</returns>
    public long GetLong(string key, long defValue) {
      string v = child.GetString(key, null);
      return v != null ? long.Parse(Decrypt(v)) : defValue;
    }

    /// <summary>
    /// Retrieve a string value from the preferences.
    /// </summary>
    /// <param name='key'>The name of the preference to retrieve.</param>
    /// <param name='defValue'>Value to return if this preference does not exist.</param>
    /// <returns>The string.</returns>
    public string GetString(string key, string defValue) {
      string v = child.GetString(key, null);
      return v != null ? Decrypt(v) : defValue;
    }

    /// <summary>
    /// Checks whether the preferences contains a preference.
    /// </summary>
    /// <param name='key'>The name of the preference to check.</param>
    /// <param name='s'>If set to <c>true</c> s.</param>
    public bool Contains(string s) {
      return child.Contains(s);
    }

    /// <summary>
    /// Registers the on shared preference change listener.
    /// </summary>
    /// <param name='onSharedPreferenceChangeListener'>On shared preference change listener.</param>
    public void RegisterOnSharedPreferenceChangeListener(ISharedPreferencesOnSharedPreferenceChangeListener onSharedPreferenceChangeListener) {
      child.RegisterOnSharedPreferenceChangeListener(onSharedPreferenceChangeListener);
    }

    /// <summary>
    /// Unregisters the on shared preference change listener.
    /// </summary>
    /// <param name='onSharedPreferenceChangeListener'>On shared preference change listener.</param>
    public void UnregisterOnSharedPreferenceChangeListener(ISharedPreferencesOnSharedPreferenceChangeListener onSharedPreferenceChangeListener) {
      child.UnregisterOnSharedPreferenceChangeListener(onSharedPreferenceChangeListener);
    }

    /// <summary>
    /// Encrypt the specified value.
    /// </summary>
    /// <param name='value'>Value.</param>
    protected static string Encrypt(string value) {
      if(string.IsNullOrEmpty(value)) {
        throw new ArgumentNullException("plainText");
      }

      string outStr = null;                       // Encrypted string to return
      RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

      try {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SEKRIT, yummy);

        // Create a RijndaelManaged object
        aesAlg = new RijndaelManaged();
        aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

        // Create a decrytor to perform the stream transform.
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using(MemoryStream msEncrypt = new MemoryStream()) {
          // prepend the IV
          msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
          msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
          using(CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
            using(StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
              //Write all data to the stream.
              swEncrypt.Write(value);
            }
          }
          outStr = Convert.ToBase64String(msEncrypt.ToArray());
        }
      }
      finally {
        // Clear the RijndaelManaged object.
        if(aesAlg != null){
          aesAlg.Clear();
        }
      }

      // Return the encrypted bytes from the memory stream.
      return outStr;
    }

    /// <summary>
    /// Decrypt the specified value.
    /// </summary>
    /// <param name='value'>Value.</param>
    protected static string Decrypt(string value) {
      if(string.IsNullOrEmpty(value)) {
        throw new ArgumentNullException("cipherText");
      }

      // Declare the RijndaelManaged object
      // used to decrypt the data.
      RijndaelManaged aesAlg = null;

      // Declare the string used to hold
      // the decrypted text.
      string plaintext = null;

      try {
        // generate the key from the shared secret and the salt
        Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(SEKRIT, yummy);

        // Create the streams used for decryption.                
        byte[] bytes = Convert.FromBase64String(value);
        using(MemoryStream msDecrypt = new MemoryStream(bytes)) {
          // Create a RijndaelManaged object
          // with the specified key and IV.
          aesAlg = new RijndaelManaged();
          aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
          // Get the initialization vector from the encrypted stream
          aesAlg.IV = readByteArray(msDecrypt);
          // Create a decrytor to perform the stream transform.
          ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
          using(CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
            using(StreamReader srDecrypt = new StreamReader(csDecrypt)){

              // Read the decrypted bytes from the decrypting stream
              // and place them in a string.
              plaintext = srDecrypt.ReadToEnd();
            }
          }
        }
      }
      finally {
        // Clear the RijndaelManaged object.
        if(aesAlg != null){
          aesAlg.Clear();
        }
      }

      return plaintext;
    }

    /// <summary>
    /// Reads the byte array.
    /// </summary>
    /// <param name='s'>S.</param>
    /// <returns>The byte array. </returns>
    private static byte[] readByteArray(Stream s) {
      byte[] rawLength = new byte[sizeof(int)];
      if(s.Read(rawLength, 0, rawLength.Length) != rawLength.Length) {
        throw new SystemException("Stream did not contain properly formatted byte array");
      }

      byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
      if(s.Read(buffer, 0, buffer.Length) != buffer.Length) {
        throw new SystemException("Did not read byte array properly");
      }

      return buffer;
    }

    /// <summary>
    /// Retrieve all values from the preferences.
    /// </summary>
    /// <value>All.</value>
    public IDictionary<string, object> All {
      get {
        return child.All;
      }
    }

    #endregion

    #region nested class

    ///<summary>
    /// Editor for the shared preferences
    /// </summary>
    public class EncryptedEditor : Java.Lang.Object, ISharedPreferencesEditor {

      /// <summary>
      /// The child editor.
      /// </summary>
      protected ISharedPreferencesEditor childEditor = null;

      /// <summary>
      /// Initializes a new instance of the <see cref="PortaPodder.ObscuredSharedPreferences+Editor"/> class.
      /// </summary>
      public EncryptedEditor(EncryptedPreferences parent) {
        this.childEditor = parent.child.Edit();
      }

      /// <summary>
      /// Puts the boolean.
      /// </summary>
      /// <param name='key'>The name of the preference to modify.</param>
      /// <param name='value'>The new value for the preference.</param>
      /// <returns>The boolean.</returns>
      public ISharedPreferencesEditor PutBoolean(string key, bool value) {
        childEditor.PutString(key, EncryptedPreferences.Encrypt(value.ToString()));
        return this;
      }

      /// <summary>
      /// Puts the float.
      /// </summary>
      /// <param name='key'>The name of the preference to modify.</param>
      /// <param name='value'>The new value for the preference.</param>
      /// <returns>The float.</returns>
      public ISharedPreferencesEditor PutFloat(string key, float value) {
        childEditor.PutString(key, EncryptedPreferences.Encrypt(value.ToString()));
        return this;
      }

      /// <summary>
      /// Puts the int.
      /// </summary>
      /// <param name='key'>The name of the preference to modify.</param>
      /// <param name='value'>The new value for the preference.</param>
      /// <returns>The int.</returns>
      public ISharedPreferencesEditor PutInt(string key, int value) {
        childEditor.PutString(key, EncryptedPreferences.Encrypt(value.ToString()));
        return this;
      }

      /// <summary>
      /// Puts the long.
      /// </summary>
      /// <param name='key'>The name of the preference to modify.</param>
      /// <param name='value'>The new value for the preference.</param>
      /// <returns>The long.</returns>
      public ISharedPreferencesEditor PutLong(string key, long value) {
        childEditor.PutString(key, EncryptedPreferences.Encrypt(value.ToString()));
        return this;
      }

      /// <summary>
      /// Puts the string.
      /// </summary>
      /// <param name='key'>The name of the preference to modify.</param>
      /// <param name='value'>The new value for the preference.</param>
      /// <returns>The string.</returns>
      public ISharedPreferencesEditor PutString(string key, string value) {
        childEditor.PutString(key, EncryptedPreferences.Encrypt(value));
        return this;
      }

      /// <summary>
      /// Note that when committing back to the preferences, the clear is done first, regardless of whether you called
      /// clear before or after put methods on this editor.
      /// </summary>
      public ISharedPreferencesEditor Clear() {
        childEditor.Clear();
        return this;
      }

      /// <summary>
      /// Commit this instance.
      /// </summary>
      public bool Commit() {
        return childEditor.Commit();
      }

      /// <summary>
      /// Note that when committing back to the preferences, all removals are done first, regardless of whether you
      /// called remove before or after put methods on this editor.
      /// </summary>
      /// <param name='key'>The name of the preference to remove.</param>
      /// <param name='s'>S.</param>
      public ISharedPreferencesEditor Remove(string s) {
        childEditor.Remove(s);
        return this;
      }
    }

    #endregion

  }
}

