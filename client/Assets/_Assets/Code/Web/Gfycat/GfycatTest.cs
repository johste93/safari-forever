using UnityEngine;
using Newtonsoft.Json;

public class Gfycat
{
    public static void UploadGif(byte[] gifBytes)
    {
        GfycatAPI.RequestAccessToken((success1, accessTokenResponse)=>
        {
            if(!success1)
                return;

            Debug.Log( JsonConvert.SerializeObject(accessTokenResponse) );

            string accessToken = accessTokenResponse.access_token;
            

            GfycatAPI.RequestUploadKey(accessToken, (success2, uploadKeyResponse)=>
            {
                if(!success2)
                    return;

                Debug.Log( JsonConvert.SerializeObject(uploadKeyResponse) );

                string uploadKey = uploadKeyResponse.gfyname;

                GfycatAPI.UploadFile(uploadKey, gifBytes,
                (success3)=>
                {
                    if(!success3)
                        return;

                    GfycatAPI.RequestUploadStatus(uploadKey, (success4, statusResponse)=>
                    {
                        if(!success4)
                            return;

                        Debug.Log( JsonConvert.SerializeObject(statusResponse) );

                        GfycatAPI.GetGfycat(uploadKey, (success5, gfycatResponse)=>
                        {
                            if(!success5)
                                return;

                            Debug.Log( JsonConvert.SerializeObject(gfycatResponse) );
                        });
                    });
                });
            });
        });
    }

    public static void Test(string uploadKey)
    {
        GfycatAPI.RequestUploadStatus(uploadKey, (success4, statusResponse)=>
        {
            if(!success4)
                return;

            Debug.Log( JsonConvert.SerializeObject(statusResponse) );

            GfycatAPI.GetGfycat(uploadKey, (success5, gfycatResponse)=>
            {
                if(!success5)
                    return;

                Debug.Log( JsonConvert.SerializeObject(gfycatResponse) );
            });
        });
    }
}
