using System.Collections.Generic;
using Farm.Gameplay.Capsules;
using UnityEngine;

public class CapsuleManager : MonoBehaviour
{
    [SerializeField] private List<Capsule> _capsules;

    public List<Capsule> Capsules => _capsules;
}