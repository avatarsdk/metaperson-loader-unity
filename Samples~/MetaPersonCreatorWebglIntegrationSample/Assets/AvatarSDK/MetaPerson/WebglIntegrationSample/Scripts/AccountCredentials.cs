using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AvatarSDK.MetaPerson.WebglIntegrationSample
{
    public class AccountCredentials : MonoBehaviour
    {
        public string clientId;

        public string clientSecret;

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret);
        }
    }
}