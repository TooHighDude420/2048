using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Entetie : MonoBehaviour
{
    

    GameObject manager;
    GameManager game;
    List<GameObject> list;
    
    private Vector2 mousePos;
    private float offsetX, offsetY;
    private const string Threek = GameManager.Threek;
    private const string Cube = GameManager.Cube;
    private const string Thrat = GameManager.Thrat;
    private const string Cube2 = GameManager.Cube2;
    private const string vijf = GameManager.vijf;
    private const string zes = GameManager.zes;
    private const string zeven = GameManager.zeven;
    public static bool mouseButtonReleased;
 

    public void Start()
    {
        manager = GameObject.Find("GameManager");
        game = manager.GetComponent<GameManager>(); 
        list = game.Enteties;
    }

    void Touch()
    {
        
    }

    private void OnMouseDown()
    {
        mouseButtonReleased = false;
        offsetX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        offsetY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }

    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(mousePos.x - offsetX, mousePos.y - offsetY);
    }

    private void OnMouseUp()
    {
        mouseButtonReleased = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        string thisGameobjectName;
        string collisionGameobjectName;

        thisGameobjectName = gameObject.name;
        collisionGameobjectName = collision.gameObject.name;

        if (mouseButtonReleased && thisGameobjectName == "Threek(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[1], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(Cube);
            game.addExp(1f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        
        else if (mouseButtonReleased && thisGameobjectName == "Cube(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[2], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(Thrat);
            game.addExp(2f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        
        else if (mouseButtonReleased && thisGameobjectName == "Trhat(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[3], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(Cube2);
            game.addExp(3f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        
        else if (mouseButtonReleased && thisGameobjectName == "Cube2(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[4], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(vijf);
            game.addExp(4f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        
        else if (mouseButtonReleased && thisGameobjectName == "5(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[5], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(zes);
            game.addExp(5f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        
        else if (mouseButtonReleased && thisGameobjectName == "6(Clone)" && thisGameobjectName == collisionGameobjectName)
        {
            Instantiate(list[6], transform.position, Quaternion.identity);
            mouseButtonReleased = false;
            game.getSpawned(zeven);
            game.addExp(6f);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
