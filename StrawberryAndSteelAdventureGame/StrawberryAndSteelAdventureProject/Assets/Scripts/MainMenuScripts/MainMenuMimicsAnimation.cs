using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MainMenuMimicsAnimation : MonoBehaviour {
 [SerializeField] private RuntimeAnimatorController mimicAnimator; private const float MinWaitTime=10f,MaxWaitTime=16f;private const int MaxMimicCount=10;private readonly Queue<GameObject> mimics=new Queue<GameObject>();
 private void Start()=>StartCoroutine(SpawnMimics());
 private IEnumerator SpawnMimics(){while(true){yield return new WaitForSeconds(Random.Range(MinWaitTime,MaxWaitTime));if(mimicAnimator==null)continue;if(mimics.Count>=MaxMimicCount){GameObject oldest=mimics.Dequeue();if(oldest!=null)Destroy(oldest);}GameObject mimic=new GameObject("Mimic");mimic.transform.SetParent(transform,false);mimic.transform.localPosition=new Vector3(-22f,-7f,0f);mimic.transform.localScale=new Vector3(5f,5f,1f);mimic.AddComponent<SpriteRenderer>();Animator a=mimic.AddComponent<Animator>();a.runtimeAnimatorController=mimicAnimator;mimic.AddComponent<MainMenuMimicAnimation>();mimics.Enqueue(mimic);}}
}
