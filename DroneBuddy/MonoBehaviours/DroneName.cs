using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

public class DroneName : MonoBehaviour
{
    public string[] potentialNames = new[]
    {
        "Frank",
        "Cramslam",
        "Fitzgerald",
        "Schfitzgerald",
        "Lemony",
        "Sally",
        "Greg",
        "Johnny",
        "C.D. Lither",
        "The Bucket Lord",
        "Lucy",
        "Truffle",
        "Bob",
        "Nathan",
        "Boviulad",
        "Cynthia",
        "Grolla",
        "Kookoo",
        "Tobey",
        "Gamer Gamerson",
        "Basket",
        "Melon Lord",
        "Gary",
        "Mortimer",
        "Lilly",
    };
    public string Name {  get; private set; }
    private void Awake()
    {
        Name = potentialNames[Random.Range(0, potentialNames.Length)];
    }
}
