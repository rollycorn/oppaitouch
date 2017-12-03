/***************************************************
Unity Chanのおっぱいを揉むスクリプト
・通常時は物理演算で揺らす
・D&Dで揉む
****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class oppaitouch : MonoBehaviour{
	public float mass = 0.8f;

	private Vector2 first_screen_pos;
	private Vector3 first_angle_r, first_angle_l;
	private bool is_touching = false;
	private Vector3 cur_angle_r, cur_angle_l;
	private GameObject oppai0, r_oppai0, l_oppai0, r_oppai1, l_oppai1;
	private Vector3 max_deg = new Vector3(1,25,10); // (-,横,縦)
	private GameObject r_oppaiX2, l_oppaiX2;
	private Vector3 root_to_r_oppai0, root_to_l_oppai0;

	// Use this for initialization
	void Start () {
		oppai0 = GameObject.Find("/unitychan/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/J_Mune_root_00");
		r_oppai0 = GameObject.Find("/unitychan/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/J_Mune_root_00/J_R_Mune_00");
		l_oppai0 = GameObject.Find("/unitychan/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/J_Mune_root_00/J_L_Mune_00");
		r_oppai1 = GameObject.Find("/unitychan/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/J_Mune_root_00/J_R_Mune_00/J_R_Mune_01");
		l_oppai1 = GameObject.Find("/unitychan/Character1_Reference/Character1_Hips/Character1_Spine/Character1_Spine1/J_Mune_root_00/J_L_Mune_00/J_L_Mune_01");
		r_oppaiX2 = new GameObject( "r_oppaiX2" );
		r_oppaiX2.transform.position = r_oppai0.transform.position + ( r_oppai1.transform.position - r_oppai0.transform.position ) * 1.2f;
		r_oppaiX2.transform.rotation = r_oppai0.transform.rotation;
		l_oppaiX2 = new GameObject( "l_oppaiX2" );
		l_oppaiX2.transform.position = l_oppai0.transform.position + ( l_oppai1.transform.position - l_oppai0.transform.position ) * 1.2f;
		l_oppaiX2.transform.rotation = l_oppai0.transform.rotation;
		root_to_r_oppai0 = r_oppai0.transform.position - oppai0.transform.position;
		root_to_l_oppai0 = l_oppai0.transform.position - oppai0.transform.position;
		_makeRigidbodies();
		_makeJoints();
	}

	void Update() {
		if( Input.GetMouseButtonDown(0) ){
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if( !Physics.Raycast(ray.origin, ray.direction*10, out hit) ){
				is_touching = false;
				return;
			}
		
			GameObject target = hit.collider.gameObject;
			if( target.name.CompareTo("unitychan") != 0 ){
				is_touching = false;
				return;
			}
		
			if( !is_touching ){
				first_screen_pos = Input.mousePosition;
				first_angle_r = r_oppai0.transform.eulerAngles;
				first_angle_l = l_oppai0.transform.eulerAngles;
			}
			is_touching = true;

			r_oppai0.transform.eulerAngles = _pix2deg( (Vector2)Input.mousePosition, first_screen_pos, Vector3.up, -Vector3.forward, max_deg, first_angle_r );
			l_oppai0.transform.eulerAngles = _pix2deg( (Vector2)Input.mousePosition, first_screen_pos, -Vector3.up, -Vector3.forward, max_deg, first_angle_l );
		}

		if( Input.GetMouseButtonUp(0) ){
			is_touching = false;
		}
		
		if( is_touching ){
			r_oppai0.transform.eulerAngles = _pix2deg( (Vector2)Input.mousePosition, first_screen_pos, Vector3.up, -Vector3.forward, max_deg, first_angle_r );
			l_oppai0.transform.eulerAngles = _pix2deg( (Vector2)Input.mousePosition, first_screen_pos, -Vector3.up, -Vector3.forward, max_deg, first_angle_l );
		}

		// 前後の動きを緩和（oppai0の前後位置を固定）
		Vector3 r_pos = r_oppai0.transform.position - oppai0.transform.position;
		Vector3 l_pos = l_oppai0.transform.position - oppai0.transform.position;
		r_oppai0.transform.position -= oppai0.transform.forward * Vector3.Dot( oppai0.transform.forward, r_pos - root_to_r_oppai0 );
		l_oppai0.transform.position -= oppai0.transform.forward * Vector3.Dot( oppai0.transform.forward, l_pos - root_to_l_oppai0 );
	}

	// 揉み幅：ひとまずD&Dの移動量[pix]と同じ角度[deg]とする
	Vector3 _pix2deg( Vector2 pix, Vector2 pix0, Vector3 k_x, Vector3 k_y, Vector3 limit, Vector3 deg0 ){
		Vector2 mov = pix - pix0;
		return deg0 + _limitRoundVector3( mov.x * k_x + mov.y * k_y, limit );
	}

	// 揉み幅を楕球領域（実際は楕円）に制限
	Vector3 _limitRoundVector3( Vector3 vec, Vector3 lim ){
		Vector3 vec2 = new Vector3( 0, 0, 0 );
		vec2.x = vec.x / lim.x;
		vec2.y = vec.y / lim.y;
		vec2.z = vec.z / lim.z;
		vec2 = vec2.magnitude > 1 ? vec2.normalized : vec2;
		vec2.x *= lim.x;
		vec2.y *= lim.y;
		vec2.z *= lim.z;
		return vec2;
	}

	void _makeRigidbodies(){
		// 剛体作成
		if( (Rigidbody)oppai0.GetComponent<Rigidbody>() == null ){
			oppai0.AddComponent<Rigidbody>();
		}
		oppai0.GetComponent<Rigidbody>().useGravity = false;
		oppai0.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;

		if( (Rigidbody)r_oppai0.GetComponent<Rigidbody>() == null ){
			r_oppai0.AddComponent<Rigidbody>();
		}
		r_oppai0.GetComponent<Rigidbody>().useGravity = false;

		if( (Rigidbody)l_oppai0.GetComponent<Rigidbody>() == null ){
			l_oppai0.AddComponent<Rigidbody>();
		}
		l_oppai0.GetComponent<Rigidbody>().useGravity = false;

		if( (Rigidbody)r_oppai1.GetComponent<Rigidbody>() == null ){
			r_oppai1.AddComponent<Rigidbody>();
		}
		r_oppai1.GetComponent<Rigidbody>().useGravity = false;

		if( (Rigidbody)l_oppai1.GetComponent<Rigidbody>() == null ){
			l_oppai1.AddComponent<Rigidbody>();
		}
		l_oppai1.GetComponent<Rigidbody>().useGravity = false;

		if( (Rigidbody)r_oppaiX2.GetComponent<Rigidbody>() == null ){
			r_oppaiX2.AddComponent<Rigidbody>();
		}
		r_oppaiX2.GetComponent<Rigidbody>().useGravity = false;

		if( (Rigidbody)l_oppaiX2.GetComponent<Rigidbody>() == null ){
			l_oppaiX2.AddComponent<Rigidbody>();
		}
		l_oppaiX2.GetComponent<Rigidbody>().useGravity = false;
	}

	void _makeJoints(){
		// 関節作成
		if( (FixedJoint)oppai0.GetComponent<FixedJoint>() == null ){
			oppai0.AddComponent<FixedJoint>();
			oppai0.AddComponent<FixedJoint>();
		}
		FixedJoint[] fj = (FixedJoint[])oppai0.GetComponents<FixedJoint>();
		fj[0].connectedMassScale = mass;
		fj[1].connectedMassScale = mass;
		fj[0].connectedBody = r_oppai0.GetComponent<Rigidbody>();
		fj[1].connectedBody = l_oppai0.GetComponent<Rigidbody>();

		if( (FixedJoint)r_oppai0.GetComponent<FixedJoint>() == null ){
			r_oppai0.AddComponent<FixedJoint>();
		}
		r_oppai0.GetComponent<FixedJoint>().connectedBody = r_oppai1.GetComponent<Rigidbody>();
		r_oppai0.GetComponent<FixedJoint>().connectedMassScale = mass;

		if( (FixedJoint)l_oppai0.GetComponent<FixedJoint>() == null ){
			l_oppai0.AddComponent<FixedJoint>();
		}
		l_oppai0.GetComponent<FixedJoint>().connectedBody = l_oppai1.GetComponent<Rigidbody>();
		l_oppai0.GetComponent<FixedJoint>().connectedMassScale = mass;

		if( (FixedJoint)r_oppai1.GetComponent<FixedJoint>() == null ){
			r_oppai1.AddComponent<FixedJoint>();
		}
		r_oppai1.GetComponent<FixedJoint>().connectedBody = r_oppaiX2.GetComponent<Rigidbody>();
		r_oppai1.GetComponent<FixedJoint>().connectedMassScale = mass;

		if( (FixedJoint)l_oppai1.GetComponent<FixedJoint>() == null ){
			l_oppai1.AddComponent<FixedJoint>();
		}
		l_oppai1.GetComponent<FixedJoint>().connectedBody = l_oppaiX2.GetComponent<Rigidbody>();
		l_oppai1.GetComponent<FixedJoint>().connectedMassScale = mass;
	}
}
