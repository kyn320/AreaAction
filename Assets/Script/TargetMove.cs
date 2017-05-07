using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMove : MonoBehaviour {

    public Vector3 target;
    Transform tr;
    public bool isMove = false, isFinish = false;

    void Awake() {
        tr = GetComponent<Transform>();
    }

    public void SetTarget(Vector3 tr) {
        target = tr;
        isMove = true;
    }

	// Update is called once per frame
	void Update () {
        if (isMove)
            tr.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 100f);

        if (!isFinish && tr.position == target) {
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
