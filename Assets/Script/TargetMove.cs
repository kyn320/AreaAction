using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour {

    public Transform target;
    Transform tr;
    public bool isMove = false, isFinish = false;

    void Awake() {
        tr = GetComponent<Transform>();
    }

    public void SetTarget(Transform tr) {
        target = tr;
        isMove = true;
        print(target.position);
    }

	// Update is called once per frame
	void Update () {
        if (isMove)
            tr.position = Vector3.MoveTowards(transform.position, target.position + (Vector3.up * 5f), Time.deltaTime * 100f);

        if (!isFinish && tr.position == target.position) {
            isFinish = true;
            StartCoroutine("Remove");
        }
	}

    IEnumerator Remove()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }

}
