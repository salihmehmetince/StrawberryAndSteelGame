using System.Collections;
using UnityEngine;
public class MainMenuMimicAnimation : MonoBehaviour {
 private const float MovementSpeed=5f,StartX=-22f,EndX=22f,LaneY=-7f; private MimicController controller;
 private void Start(){Animator a=GetComponent<Animator>();SpriteRenderer r=GetComponent<SpriteRenderer>();if(a==null||r==null){Debug.LogError("MainMenuMimicAnimation requires an Animator and SpriteRenderer.",this);enabled=false;return;}r.sortingOrder=5;controller=new MimicController(transform,a,r,MovementSpeed);StartCoroutine(WalkAcrossScreen());}
 private void Update()=>controller?.Tick(Time.deltaTime);
 private IEnumerator WalkAcrossScreen(){yield return new WaitForSeconds(Random.Range(1f,2f));bool right=Random.value<.5f;float start=right?StartX:EndX,end=right?EndX:StartX;transform.localPosition=new Vector3(start,LaneY,transform.localPosition.z);controller.WalkTo(new Vector2(end,LaneY));yield return new WaitUntil(()=>!controller.IsMoving);}
}
