using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Entetie : MonoBehaviour
{
    GameObject manager;
    GameManager game;
    List<GameObject> Entlist;
    public bool touched = false;
    private const string Threek = GameManager.Threek;
    private const string Cube = GameManager.Cube;
    private const string Thrat = GameManager.Thrat;
    private const string Cube2 = GameManager.Cube2;
    private const string vijf = GameManager.vijf;
    private const string zes = GameManager.zes;
    private const string zeven = GameManager.zeven;
    public static bool mouseButtonReleased;
    Transform selected;
    float dist;
    Vector3 Offset;
    BoxCollider2D collider2D;
    int Height, Width;
    float X, Y, speed;
    Vector2 Destination;
    bool gotDestination = false;
    Rigidbody2D spritePos;
    int t = 0;

    public void Start()
    {
        manager = GameObject.Find("GameManager");
        game = manager.GetComponent<GameManager>();
        Entlist = game.Enteties;
        collider2D = GetComponent<BoxCollider2D>();
        Height = Screen.height;
        Width = Screen.width;
        spritePos = GetComponent<Rigidbody2D>();
        speed = 200f;
        game.bugSquasher("Start Executed on Entetie");
    }
    
    void Update()
    {
        Vector3 v3;

        if (Input.touchCount == 0)
        {
            game.bugSquasher("No Touch");
            touched = false;
            return;
        }

        Touch touch = Input.GetTouch(0);
        Vector2 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
           game.bugSquasher("Touch began");
            
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

            if (hit.collider != null)
            {
                game.bugSquasher("hit something");
                if (hit.collider.gameObject.name != null)
                {
                    selected = hit.transform;
                    game.bugSquasher("hit a gameobject with a name");
                    dist = selected.position.z;
                    v3 = new Vector2(pos.x, pos.y);
                    v3 = Camera.main.ScreenToWorldPoint(v3);
                    Offset = selected.position - v3;
                    touched = true;
                }
            }
        }

        if (touched && touch.phase == TouchPhase.Moved)
        {
            game.bugSquasher("Touch moved");
            v3 = new Vector3(touch.position.x, touch.position.y, dist);
            v3 = Camera.main.ScreenToWorldPoint(v3);
            selected.position = v3 + Offset;
        }

        if (touched && touch.phase == TouchPhase.Ended)
        {
            

            List<Collider2D> colliders = new();
            ContactFilter2D contactFilter2D = new();
            collider2D.OverlapCollider(contactFilter2D, colliders);
            if (colliders.Count > 0)
            {
                game.bugSquasher("dropped and collided");
                int indexOfColliders;
                for (int i = 0; i < colliders.Count; i++)
                {
                    game.bugSquasher("collided with " + i.ToString() + " Objects");
                    string tmp = colliders[i].gameObject.name;
                    if (tmp == selected.name)
                    {
                        game.bugSquasher("collided with a object with the same name");
                        indexOfColliders = i;
                        GameObject temp = new();
                        temp.name = removePartOfString(selected.name, "(Clone)");
                        int indexList = Entlist.IndexOf(temp);
                        game.bugSquasher("Index of object in list: " + indexList);
                        Destroy(temp);
                        indexList++;
                        Instantiate(Entlist[indexList], selected.position, Quaternion.identity);
                        mouseButtonReleased = false;
                        game.getSpawned(Entlist[indexList].name);
                        game.addExp(1);
                        Destroy(colliders[indexOfColliders].gameObject);
                        Destroy(gameObject);
                        break;
                    }
                }

            }

            touched = false;

        }
    }

    void FixedUpdate()
    {
        if (!gotDestination)
        {
            Destination = GetDestination();
        }
        if (gotDestination)
        {
            {
                Destination = Destination.normalized;
                Vector2 move = Destination * speed * Time.deltaTime;
                spritePos.AddForce(move);
                gotDestination = false;

            }
        }
    }

    string removePartOfString(string originalString, string substringToRemove)
    {
        int index = originalString.IndexOf(substringToRemove);

        if(index != -1)
        {
            string resultString = originalString.Remove(index, substringToRemove.Length);
            return resultString;
        }
        else
        {
            return "error";
        }
    }
   

    Vector2 GetDestination()
    {
        Y = Random.Range(0, Height);
        X = Random.Range(0, Width);

        Vector2 des = new Vector2(X, Y);
        gotDestination = true;

        return des -= spritePos.position;
    }

}
