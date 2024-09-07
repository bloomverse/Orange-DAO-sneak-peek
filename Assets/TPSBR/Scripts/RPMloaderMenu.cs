using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadyPlayerMe;


namespace TPSBR.UI
{
    public class RPMloaderMenu : UICloseView
    {
        // Start is called before the first frame update
    [SerializeField]
    GameObject avatarPlaceholder;

    [SerializeField]
    RuntimeAnimatorController animatorController;

    [SerializeField]
    Avatar animatorAvatar;

    [SerializeField]
    GameObject gun;



    private AvatarLoader avatarLoader;
    private GameObject loaded;
     
    private string avatarDef =  Constants.RPM_defaultURL;
                                 //"https://api.readyplayer.me/v1/avatars/63b532d2f17e295642bee041.glb";


        private void Start(){
           // PlayerPrefs.DeleteAll();
            if(PlayerPrefs.GetString("RPMURL")!=null && PlayerPrefs.GetString("RPMURL").Length>10){
                Debug.Log("Loading from playerrefs" + PlayerPrefs.GetString("RPMURL"));
                LoadAvatar(PlayerPrefs.GetString("RPMURL"));
               
            }else{
                Debug.Log("Loading default");
                LoadAvatar(avatarDef);
                
            }
        }



        public void LoadAvatar(string avatarURL)
        {
            
            
            Debug.Log($"Started loading avatar " + avatarURL);
             avatarLoader = new AvatarLoader();
            
            avatarLoader.OnCompleted += AvatarLoadComplete;
            avatarLoader.OnFailed += AvatarLoadFail;
            avatarLoader.LoadAvatar(avatarURL);

          

        }

        private void AvatarLoadComplete(object sender, CompletionEventArgs args)
        {

            
          
            if(loaded!=null){
                Destroy(loaded);
            }
            
            loaded = args.Avatar;
            args.Avatar.transform.parent = avatarPlaceholder.transform;


            args.Avatar.transform.position = avatarPlaceholder.transform.position;
            args.Avatar.transform.rotation = avatarPlaceholder.transform.rotation;

            Animator animator = args.Avatar.GetComponentInParent<Animator>();
            animator.avatar =  animatorAvatar;
            animator.runtimeAnimatorController = animatorController;
            animator.applyRootMotion = true;

            


           /* foreach (Transform eachChild in args.Avatar.transform) {
     
         Debug.Log ("Child found. Mame: " + eachChild.name);
     
 }*/

            var gunI =  Instantiate(gun,new Vector3(0,0,0),Quaternion.identity);

            Transform fingerBone = args.Avatar.transform.FindRecursive("RightHandMiddle2");
            Debug.Log(fingerBone + " fingerbone");

            gunI.transform.parent = fingerBone;
            gunI.transform.position = fingerBone.position;
            gunI.transform.rotation = fingerBone.rotation;
            
            //Animator animator = args.Avatar.GetComponentInParent<Animator>();

          
        }
        
        private void AvatarLoadFail(object sender, FailureEventArgs args)
        {
            Debug.Log($"Avatar loading failed with error message: {args.Message}");

          
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}