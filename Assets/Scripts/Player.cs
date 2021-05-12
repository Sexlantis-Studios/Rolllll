using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 1f;

    public float cameraFollow = 0.75f;
    public float slideScale;
    public float midpoint;
    public Vector3 cameraOffset;

    public LayerMask ignoreAimPlane;

    bool isAlive = true;

    public Button retryButton;
    public GameObject loseScreen;

    public float jumpPower;

    void Start() {


        retryButton.onClick.AddListener(Retry);
    }

    // Update is called once per frame
    void Update() {

        Forward(speed);

        Movement();

        if (isAlive) {
            Cursor.visible = false;
            loseScreen.SetActive(false);
            CameraTrack();
        } else if (!loseScreen.activeSelf) {
            StartCoroutine(showFailScreenAfterDelay(0.8f));
        }

    }

    void Forward(float move) {
        //move forward at the set speed
        transform.Translate(new Vector3(0, 0, move * Time.deltaTime));
    }

    void Movement() {

        if (isAlive) {
            //move to where the mouse is pointing
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, ignoreAimPlane)) {
                transform.position = new Vector3((hit.point.x + midpoint) / slideScale, transform.position.y, transform.position.z);
            }
        }
    }

    void CameraTrack() {
        Vector3 pos = new Vector3((transform.position.x), 0, transform.position.z) + cameraOffset;
        Camera.main.transform.position = pos;
    }


    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "KillPlane") {
            isAlive = false;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    void Retry() {
        SceneManager.LoadScene("Level1");
    }

    IEnumerator showFailScreenAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Cursor.visible = true;
        loseScreen.SetActive(true);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Jump") {
            other.gameObject.SetActive(false);
            //jump, the function of the jump for 4 tiles and a height of 2.5 is h(x) = -0.625(x^2 -4x)
            StartCoroutine(Jump());
        }
    }

    IEnumerator Jump() {
        float startHeight = transform.position.y;
        float startPos = transform.position.z;
        while (transform.position.z - startPos < 4.0f) {
            double y = -0.625 * (Mathf.Pow(transform.position.z,2) - (-4 * transform.position.z)); //h(x) = -0.625(x^2 -4x)
            y = Mathf.Clamp((float)y, startHeight, 3f);
            transform.position = new Vector3(transform.position.x,(float)y,transform.position.z);
            yield return null; //wait one frame
        }
    }


}