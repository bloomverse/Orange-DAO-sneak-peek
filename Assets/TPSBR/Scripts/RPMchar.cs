using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using ReadyPlayerMe;
using UnityEngine;

using TPSBR;
using Fusion;


    public class RPMchar : MonoBehaviour
    {
        // Start is called before the first frame update
    [SerializeField]
    GameObject avatarPlaceholder;

   
    [SerializeField]
    GameObject  SMRObject;


    [SerializeField]
    SkinnedMeshRenderer srender;

    [SerializeField]
    GameObject  charContainer;

    [SerializeField]
    Transform rootBone;




    // [SerializeField]
        //private string avatarURL = "https://api.readyplayer.me/v1/avatars/63b1bd6dd16b67196c5b3d08.glb";

        public void loadAv(string avatarURL)
        {
            Debug.Log($"Started loading avatar");
            AvatarLoader avatarLoader = new AvatarLoader();
            
            avatarLoader.OnCompleted += AvatarLoadComplete;
            avatarLoader.OnFailed += AvatarLoadFail;
            avatarLoader.LoadAvatar(avatarURL);
        }

        

        private void AvatarLoadComplete(object sender, CompletionEventArgs args)
        {
            Debug.Log("loaded avatar ");
            args.Avatar.transform.parent = avatarPlaceholder.transform;
            args.Avatar.transform.position = avatarPlaceholder.transform.position;
            args.Avatar.transform.rotation = avatarPlaceholder.transform.rotation;

            Transform armature = args.Avatar.transform.FindRecursive("Armature");
            Transform hips = args.Avatar.transform.FindRecursive("Hips");

            

            Transform spine = args.Avatar.transform.FindRecursive("Spine");
            Transform spine1 = args.Avatar.transform.FindRecursive("Spine1");
            Transform spine2 = args.Avatar.transform.FindRecursive("Spine2");
            Transform righthandle = args.Avatar.transform.FindRecursive("RightHandMiddle2");


            //Moving parts
            
            //Move Granades
            Transform granade1 = charContainer.transform.FindRecursive("GrenadeHandle1");
            granade1.transform.parent = spine2.transform;
            granade1.transform.localPosition = new Vector3(0.068f,-0.348f,0.146f);

            Transform granade2 = charContainer.transform.FindRecursive("GrenadeHandle2");
            granade2.transform.parent = spine2.transform;
            granade2.transform.localPosition = spine2.transform.localPosition;
            Transform granade3 = charContainer.transform.FindRecursive("GrenadeHandle3");
            granade3.transform.parent = spine2.transform;
            granade3.transform.localPosition = spine2.transform.localPosition;
            //Move BackHandle
            Transform backhandle = charContainer.transform.FindRecursive("BackHandle");
            backhandle.transform.parent = spine1.transform;
            backhandle.transform.localPosition = new Vector3(-0.018f,0.312f,-0.086f);
            //backhandle.transform.localPosition = Vector3.zero;
            //Move Belt Handle
            Transform belthandle = charContainer.transform.FindRecursive("BeltHandle");
            belthandle.transform.parent = spine.transform;
            belthandle.transform.localPosition = new Vector3(.150f,0,0);
            //Move W1 Handle
            Transform rightW1 = charContainer.transform.FindRecursive("WeaponHandlePistol");
            rightW1.transform.parent = righthandle.transform;
            rightW1.transform.localPosition = new Vector3(0.020f,-0.027f,-0.0224f);
            //Movie w2 Handle
             Transform rightW2 = charContainer.transform.FindRecursive("WeaponHandleRifle");
            rightW2.transform.parent = righthandle.transform;
            rightW2.transform.localPosition= new Vector3(0.025f,-0.03f, 0.005f);
            // Create Avatar 
            Animator animObject = args.Avatar.GetComponent<Animator>();
             //animObject.applyRootMotion = false;

            GameObject rootGO = args.Avatar;
            //rootGO.transform.rotation = Quaternion.identity;
            Avatar avatar = AvatarBuilder.BuildGenericAvatar(rootGO,"");
            
            avatar.name = "GenericImportedRPM";
            animObject.avatar = avatar;
            armature.transform.Rotate(-90,0,0);

            // Character Armatura
            var charView = GetComponentInParent<Character>()._thirdPersonView;
            charView.RootBone = args.Avatar.transform.FindRecursive("Armature");
            charView.HeadTransform = args.Avatar.transform.FindRecursive("Head");
            charView.LeftFoot = args.Avatar.transform.FindRecursive("LeftFoot");
            charView.RightFoot = args.Avatar.transform.FindRecursive("RightFoot");

            // Character Animation controller
            var  chara = GetComponentInParent<CharacterAnimationController>();
           

            chara.SetAnimator(animObject);
            //chara.Animator = animObject;

          //  chara._leftHand =  args.Avatar.transform.FindRecursive("LeftHand");
          //  chara._leftLowerArm =  args.Avatar.transform.FindRecursive("LeftForeArm");
          //  chara._leftUpperArm =  args.Avatar.transform.FindRecursive("LeftArm");


            
            spine2.transform.gameObject.AddComponent(charContainer.transform.FindRecursive("Spine2").GetComponent<Hitbox>());

            args.Avatar.transform.FindRecursive("Head").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("Head").GetComponent<Hitbox>());

            args.Avatar.transform.FindRecursive("RightArm").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("RightArm").GetComponent<Hitbox>());
            args.Avatar.transform.FindRecursive("RightForeArm").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("RightForeArm").GetComponent<Hitbox>());

            args.Avatar.transform.FindRecursive("RightUpLeg").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("RightUpLeg").GetComponent<Hitbox>());
            args.Avatar.transform.FindRecursive("RightLeg").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("RightLeg").GetComponent<Hitbox>());

             args.Avatar.transform.FindRecursive("LeftArm").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("LeftArm").GetComponent<Hitbox>());
            args.Avatar.transform.FindRecursive("LeftForeArm").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("LeftForeArm").GetComponent<Hitbox>());

            args.Avatar.transform.FindRecursive("LeftUpLeg").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("LeftUpLeg").GetComponent<Hitbox>());
            args.Avatar.transform.FindRecursive("LeftLeg").transform.gameObject.AddComponent(charContainer.transform.FindRecursive("LeftLeg").GetComponent<Hitbox>());

           

            charContainer.SetActive(false);



            //Hitboxes Move



            //args.Avatar.transform.localScale = new Vector3(.1f,.1f,.1f); 
            //args.Avatar.transform.localScale  += new Vector3(.1f,.1f,.1f);
            //var skin = args.Avatar.GetComponentInChildren<SkinnedMeshRenderer>();
            //skin.transform.parent = charContainer.transform;
            //skin.transform.localScale
            //srender.sharedMesh = skin.sharedMesh;
            
            
            //Debug.Log(skin.bones.Length + " Largo huesos de importado");
            //Debug.Log(srender.bones.Length + " Largo huesos de local");

         //Transform[] childrens = rootBone.transform.GetComponentsInChildren<Transform> (true);

       // ReassignBones(skin,srender);
       // skin.rootBone = rootBone;


         /*   Transform[] bones = new Transform[srender.bones.Length];
		for (int boneOrder = 0; boneOrder < srender.bones.Length; boneOrder++) {
           // Debug.Log(srender.bones [boneOrder].name);
			bones [boneOrder] = Array.Find<Transform> (childrens, c => c.name == srender.bones [boneOrder].name);
		}
		skin.bones = bones;*/

        //charContainer.transform.localScale = new Vector3(100,100,100);

            //skin.transform.localScale = new Vector3(.1f,.1f,.1f);
            //srender.sharedMesh = skin.sharedMesh;
            //srender.transform.localScale = new Vector3(.1f,.1f,.1f);
            //srender.transform.localScale += new Vector3(.1f,.1f,.1f);

            //args.Avatar.transform.setActive(false);
            //args.Avatar.setActive(false);
            //skin.rootBone = rootBone;
           /* Animator animator = args.Avatar.GetComponentInParent<Animator>();
            animator.avatar =  animatorAvatar;
            animator.runtimeAnimatorController = animatorController;
            animator.applyRootMotion = true;*/

           /*/ foreach (Transform eachChild in args.Avatar.transform) {
     
         Debug.Log ("Child found. Mame: " + eachChild.name);
     
 }*/

        

           /* Transform fingerBone = args.Avatar.transform.FindRecursive("RightHandMiddle2");
            Debug.Log(fingerBone);

            gun.transform.parent = fingerBone;
            gun.transform.position = fingerBone.position;
            gun.transform.rotation = fingerBone.rotation;*/
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

        public void ReassignBones(SkinnedMeshRenderer sourceSMR, SkinnedMeshRenderer targetSMR)
        {
    
    
        // bones don't match, need to rebuild targetSMR bones
        List<Transform> newBones = new List<Transform>();
        // add all existing bones
        newBones.AddRange(targetSMR.bones);
        // add missing bones
        foreach (Transform sourceBone in sourceSMR.bones)
        {
            bool found = false;
            foreach (Transform targetBone in targetSMR.bones)
            {
                if (sourceBone.name == targetBone.name)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                newBones.Add(sourceBone);
            }
        }
        // assign new bones
        targetSMR.bones = newBones.ToArray();
    }
}
    


