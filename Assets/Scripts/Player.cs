using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            StartCoroutine(Jump(3.8f, other));
            //boing
            other.gameObject.transform.parent.Translate(Vector3.up * 0.5f);
        }
        if (other.gameObject.tag == "KillPlane") {
            isAlive = false;
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    IEnumerator Jump(float distance, Collider other) {
        float startHeight = transform.position.y;
        float startPos = other.transform.position.z - (other.gameObject.transform.localScale.z / 2);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        while(transform.position.z - startPos < 0) {
            yield return null;
        }
        while (transform.position.z - startPos < distance) {

            //h(x) = -0.625(x^2 -4x)
            float progress = transform.position.z - startPos;

            float y = progress * progress; //x^2
            y += -distance * progress; //-4x
            y *= -0.8f; //-0.625
            y += startHeight;

            //print("y=" + y + "  progress: " + progress);

            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            yield return null; //wait one frame
        }
        rb.useGravity = true;
    }
}