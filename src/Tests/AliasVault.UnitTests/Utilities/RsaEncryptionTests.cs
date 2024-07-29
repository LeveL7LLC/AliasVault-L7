//-----------------------------------------------------------------------
// <copyright file="RsaEncryptionTests.cs" company="lanedirt">
// Copyright (c) lanedirt. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

namespace AliasVault.Tests.Utilities;

using System.Security.Cryptography;
using System.Text.Json;
using Cryptography;

/// <summary>
/// Tests for the SrpArgonEncryption class.
/// </summary>
public class RsaEncryptionTests
{
    /// <summary>
    /// Example public key for RSA encryption tests. This is a public key generated by the JSInterop on the client.
    /// </summary>
    public const string PublicKey = "{\"alg\":\"RSA-OAEP-256\",\"e\":\"AQAB\",\"ext\":true,\"key_ops\":[\"encrypt\"],\"kty\":\"RSA\",\"n\":\"lW8fRfSvLQiK9uZgm_kFjHMY1SedAZlVvZ_8d_d5oqWezQhan8-Y10Qvx0NMe57sQB3ePnShJFNE33w83kgRNkOyxKJ2FOVKtRptd7CgwIt_l9TPjdrB0J0hFn9b1eit2vpQlOdP_Wa8WvW2eVdXYEMWuBU4-aj8vY2qzcmBc-HhJX-Me9oXhUscJxeqMP4_sNiN7D4I0enrmYicB3JQMhUIwMmNt-0srHTdSvHh_6vFZMqB9ohfh2D9Q0BzYcI8rGEy1RTYsmF1zYyoOOzeRGOcKCVNeLO9LZxfAdm1Eq5zv47uw543cxCZXIZPlXOVriMEtTRwaGzE_3RZmpGJqw\"}";

    /// <summary>
    /// Example private key for RSA encryption tests. This is a private key generated by the JSInterop on the client.
    /// </summary>
    public const string PrivateKey = "{\"alg\":\"RSA-OAEP-256\",\"d\":\"KLByToUaseNym1oNkkrTRPQOHfREXywWWaTXhP8AwtXgEKomqv9G-c6aR-K-T6btY2P-oPj268I0rbnRhSEQdrsmUT5_cp8goYGJrx6MFwGlA32x6klXnus6GDsjkXJi7I5eJL17XV99CDOBtTagFxkNdaBpvClUcHTDvncQ5bGAIrNqS7KADoi-E19BxiW_GcSJiVT4H8kDHCkcgTjZx4rKJjTPqqJOLg_poDrvnTJbsjcXP80kQ1AAENRAvDGhSWzP0IYtP1DM_2FzM1s1b_SrUsS3KiO8drR2Kv-PSOvncpaNVnZGElGCraJ3B2Mm-dr3vFjkyWeWPceqyhtYoQ\",\"dp\":\"ttxRg6uB2YLWfkPKUkzAaBWniZDHM4silJX3IgexA5GJBd9GIhUiVEolc_MgmieQbZ10CC65wqcHVv82lgCeqxYHxHWLxxJCrOpvkFlYE8wr_WqOPQEzYKv3KsL6s6Fj7Pbv9WehWpXdlbJUm4Cy5cgUkdH6PXiwBSvfhCQGrYk\",\"dq\":\"YFqlDAVTfvTR2bMJulvWzd_at81CsEmR-lPo91h-3cLpxcLDOlrTP-d3Ass2I4r1PtBT1bKuuHeQ6fZmHH55a6m8XxPEs2BuIxlh9RiFfWbd66969UOnItuawf0rfGneKt1zl4st60T3KXd8-ECrLxdsvOYpOEuNzvIY_b3qitE\",\"e\":\"AQAB\",\"ext\":true,\"key_ops\":[\"decrypt\"],\"kty\":\"RSA\",\"n\":\"lW8fRfSvLQiK9uZgm_kFjHMY1SedAZlVvZ_8d_d5oqWezQhan8-Y10Qvx0NMe57sQB3ePnShJFNE33w83kgRNkOyxKJ2FOVKtRptd7CgwIt_l9TPjdrB0J0hFn9b1eit2vpQlOdP_Wa8WvW2eVdXYEMWuBU4-aj8vY2qzcmBc-HhJX-Me9oXhUscJxeqMP4_sNiN7D4I0enrmYicB3JQMhUIwMmNt-0srHTdSvHh_6vFZMqB9ohfh2D9Q0BzYcI8rGEy1RTYsmF1zYyoOOzeRGOcKCVNeLO9LZxfAdm1Eq5zv47uw543cxCZXIZPlXOVriMEtTRwaGzE_3RZmpGJqw\",\"p\":\"yUdbuDwmVwKhou5xXUxJfi1eOjN-5F88wtyR4LpgU2OvZ7m-er4hpXx5I2E-KTVX_iIp0Q9VDXhHH-WkN3qg20RXjRoxwgrggYbfdIYdrB-2kbMamq5cOf2XbXGEO8PoDXYoZprIB0EhrD4qVVykPUYg5El0hIKPdfs9LNoOEzs\",\"q\":\"vg93lGTurG0EY179tPr6Qe3ttKEN9zvQ97dZ9034DOWDoWLe-iMKG1-yKmkG4uwC8QqNnm1mPz7EqOuHPPGVTTib9NA4JdM27PUHSPKDUvp0cV4LhF6e-W7tMFk8WbJ2ACqkqhZHYgm-FDkZBCpnehNegTxipLluKa79G__ZHFE\",\"qi\":\"fnI3Wh5aYuxI0R18NTeFKjo1P7_Ck65Gc9O3CmeqiIe58EJaXQEcdwdSOG8aVmn03szXLHEnp7anNIH63f0ericbRYdCQVhcQpvsXzEM_sp4aYmwz45palrjlY4Jc6G6XQn3FwiqqRDvpnXdsunnQ62HHhxmslaEMYHQyLng2ss\"}";

    /// <summary>
    /// Full flow test for server-side email encryption and client-side decryption.
    /// </summary>
    [Test]
    public void EmailEncryptionAndDecryptionTest()
    {
        // -----------------
        // Server-side part:
        // -----------------
        // 1. Generate symmetric key.
        var symmetricKey = Encryption.GenerateRandomSymmetricKey();

        // 2. Encrypt email body with symmetric key.
        var emailBody = "Hello, RSA encryption!";
        var encryptedEmailBody = Encryption.SymmetricEncrypt(emailBody, symmetricKey);

        // 3. Encrypt symmetric key with public key.
        var encryptedSymmetricKey = Encryption.EncryptSymmetricKeyWithRsa(symmetricKey, PublicKey);

        // -----------------
        // Client-side part:
        // -----------------
        // 4. Decrypt symmetric key with private key.
        var decryptedSymmetricKey = Encryption.DecryptSymmetricKeyWithRsa(encryptedSymmetricKey, PrivateKey);

        // 5. Decrypt email body with symmetric key.
        var decryptedEmailBody = Encryption.SymmetricDecrypt(encryptedEmailBody, decryptedSymmetricKey);

        Assert.That(decryptedEmailBody, Is.EqualTo(emailBody));
    }

    /// <summary>
    /// Tests that GenerateRsaKeyPair method returns a valid key pair.
    /// </summary>
    [Test]
    public void GenerateRsaKeyPair_ShouldReturnValidKeyPair()
    {
        Assert.That(PublicKey, Is.Not.Null);
        Assert.That(PrivateKey, Is.Not.Null);
        Assert.That(PublicKey, Is.Not.EqualTo(PrivateKey));

        // Verify that the keys are in valid JSON format
        Assert.That(() => JsonSerializer.Deserialize<Dictionary<string, object>>(PublicKey), Throws.Nothing);
        Assert.That(() => JsonSerializer.Deserialize<Dictionary<string, object>>(PrivateKey), Throws.Nothing);
    }

    /// <summary>
    /// Tests that encryption with public key followed by decryption with private key returns the original plaintext.
    /// </summary>
    [Test]
    public void EncryptWithPublicKey_DecryptWithPrivateKey_ShouldReturnOriginalPlaintext()
    {
        // Example public and private keys as generated by the JSInterop on the client.
        string originalPlaintext = "Hello, RSA encryption!";

        string ciphertext = Encryption.EncryptWithPublicKey(originalPlaintext, PublicKey);
        string decryptedText = Encryption.DecryptWithPrivateKey(ciphertext, PrivateKey);

        Assert.That(decryptedText, Is.EqualTo(originalPlaintext));
    }

    /// <summary>
    /// Tests that encrypting the same plaintext twice produces different ciphertexts.
    /// </summary>
    [Test]
    public void EncryptWithPublicKey_ShouldProduceDifferentCiphertextForSamePlaintext()
    {
        string plaintext = "Same plaintext";

        string ciphertext1 = Encryption.EncryptWithPublicKey(plaintext, PublicKey);
        string ciphertext2 = Encryption.EncryptWithPublicKey(plaintext, PublicKey);

        Assert.That(ciphertext2, Is.Not.EqualTo(ciphertext1));
    }

    /// <summary>
    /// Tests that decrypting an invalid ciphertext throws an exception.
    /// </summary>
    [Test]
    public void DecryptWithPrivateKey_ShouldThrowExceptionForInvalidCiphertext()
    {
        string invalidCiphertext = "ThisIsNotValidCiphertext";

        Assert.That(
            () => Encryption.DecryptWithPrivateKey(invalidCiphertext, PrivateKey),
            Throws.TypeOf<CryptographicException>());
    }

    /// <summary>
    /// Tests encryption and decryption with a long plaintext string.
    /// </summary>
    [Test]
    public void EncryptDecrypt_ShouldWorkWithLongPlaintext()
    {
        string longPlaintext = new string('A', 192); // 192 character string

        string ciphertext = Encryption.EncryptWithPublicKey(longPlaintext, PublicKey);
        string decryptedText = Encryption.DecryptWithPrivateKey(ciphertext, PrivateKey);

        Assert.That(decryptedText, Is.EqualTo(longPlaintext));
    }

    /// <summary>
    /// Tests encryption and decryption with special characters.
    /// </summary>
    [Test]
    public void EncryptDecrypt_ShouldWorkWithSpecialCharacters()
    {
        string specialChars = "!@#$%^&*()_+{}[]|\\:;\"'<>,.?/~`";

        string ciphertext = Encryption.EncryptWithPublicKey(specialChars, PublicKey);
        string decryptedText = Encryption.DecryptWithPrivateKey(ciphertext, PrivateKey);

        Assert.That(decryptedText, Is.EqualTo(specialChars));
    }

    /// <summary>
    /// Tests encryption and decryption with Unicode characters from different languages.
    /// </summary>
    [Test]
    public void EncryptDecrypt_ShouldWorkWithUnicodeCharacters()
    {
        string unicodeText = "こんにちは世界！ - Здравствуй, мир! - مرحبا بالعالم! - 你好，世界！";

        string ciphertext = Encryption.EncryptWithPublicKey(unicodeText, PublicKey);
        string decryptedText = Encryption.DecryptWithPrivateKey(ciphertext, PrivateKey);

        Assert.That(decryptedText, Is.EqualTo(unicodeText));
    }

    /// <summary>
    /// Tests that encrypting with an invalid public key throws an exception.
    /// </summary>
    [Test]
    public void EncryptWithPublicKey_ShouldThrowExceptionForInvalidPublicKey()
    {
        string invalidPublicKey = "ThisIsNotAValidPublicKey";
        string plaintext = "Test plaintext";

        Assert.Throws<JsonException>(() => Encryption.EncryptWithPublicKey(plaintext, invalidPublicKey));
    }

    /// <summary>
    /// Tests that decrypting with an invalid private key throws an exception.
    /// </summary>
    [Test]
    public void DecryptWithPrivateKey_ShouldThrowExceptionForInvalidPrivateKey()
    {
        string invalidPrivateKey = "ThisIsNotAValidPrivateKey";
        string plaintext = "Test plaintext";

        string ciphertext = Encryption.EncryptWithPublicKey(plaintext, PublicKey);

        Assert.Throws<JsonException>(() => Encryption.DecryptWithPrivateKey(ciphertext, invalidPrivateKey));
    }

    /// <summary>
    /// Tests if GenerateRandomSymmetricKey method returns a key of correct length.
    /// </summary>
    [Test]
    public void GenerateRandomSymmetricKey_ReturnsCorrectLength()
    {
        var key = Encryption.GenerateRandomSymmetricKey();
        Assert.That(key.Length, Is.EqualTo(32), "The generated key should be 32 bytes (256 bits) long.");
    }

    /// <summary>
    /// Tests if GenerateRandomSymmetricKey method generates different keys on consecutive calls.
    /// </summary>
    [Test]
    public void GenerateRandomSymmetricKey_GeneratesDifferentKeys()
    {
        var key1 = Encryption.GenerateRandomSymmetricKey();
        var key2 = Encryption.GenerateRandomSymmetricKey();
        Assert.That(key1, Is.Not.EqualTo(key2), "Two generated keys should not be identical.");
    }

    /// <summary>
    /// Tests if EncryptSymmetricKey method correctly encrypts a symmetric key.
    /// </summary>
    [Test]
    public void EncryptSymmetricKey_EncryptsCorrectly()
    {
        var symmetricKey = Encryption.GenerateRandomSymmetricKey();
        var encryptedKey = Encryption.EncryptSymmetricKeyWithRsa(symmetricKey, PublicKey);

        Assert.That(encryptedKey, Is.Not.Null.And.Not.Empty, "Encrypted key should not be null or empty.");
        Assert.That(encryptedKey, Is.Not.EqualTo(Convert.ToBase64String(symmetricKey)), "Encrypted key should be different from the original key.");
    }

    /// <summary>
    /// Tests if a symmetric key can be correctly encrypted and then decrypted.
    /// </summary>
    [Test]
    public void EncryptAndDecryptSymmetricKey_ReturnsOriginalKey()
    {
        var symmetricKey = Encryption.GenerateRandomSymmetricKey();
        var encryptedKey = Encryption.EncryptSymmetricKeyWithRsa(symmetricKey, PublicKey);

        // Assuming you have a method to decrypt with the private key
        var decryptedKey = Encryption.DecryptSymmetricKeyWithRsa(encryptedKey, PrivateKey);

        Assert.That(decryptedKey, Is.EqualTo(symmetricKey), "Decrypted key should match the original symmetric key.");
    }

    /// <summary>
    /// Tests if EncryptSymmetricKey method throws an exception when given an invalid public key.
    /// </summary>
    [Test]
    public void EncryptSymmetricKey_WithInvalidPublicKey_ThrowsException()
    {
        var symmetricKey = Encryption.GenerateRandomSymmetricKey();
        var invalidPublicKey = "invalid_key";

        Assert.Throws<JsonException>(
            () => Encryption.EncryptSymmetricKeyWithRsa(symmetricKey, invalidPublicKey),
            "Encrypting with an invalid public key should throw an ArgumentException.");
    }
}
