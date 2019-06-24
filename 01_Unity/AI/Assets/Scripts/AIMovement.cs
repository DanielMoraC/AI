using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    //Select team
    public Team Team => _team;
    [SerializeField] private Team _team;
    [SerializeField] private LayerMask _layerMask;
    
    //Range
    [SerializeField]private float _attackRange = 3f;
    [SerializeField]private float _aggroRadius = 5f;
    private float _rayDistance = 5.0f;
    private float _stoppingDistance = 1.5f;
    
    //CoolDown
    public float attackRate;
    private float attackNextFire;
    
    //Attack damage
    [SerializeField] private int _damage = 25;
    
    //Projectile and Launcher
    public GameObject ProjectilePrefab;
    public GameObject Launcher;
    
    private Vector3 _destination;
    private Quaternion _desiredRotation;
    private Vector3 _direction;

    //Rotate the character before it starts running away
    private bool _rotate = false;

    //The target enemy
    private AIMovement _target;
    public GameObject _targetGameobject;

    //The state of the character
    [SerializeField]private DroneState _currentState;
    

    private void Update()
    {
        //Change direction
        if (_rotate)
        {
            transform.RotateAround (transform.position, transform.up, 180f);
        }
        
        switch (_currentState)
        {
            //Move around searching for an enemy
            case DroneState.Wander:
            {
                if (NeedsDestination())
                {
                    GetDestination();
                }

                transform.rotation = _desiredRotation;

                transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                var rayColor = IsPathBlocked() ? Color.red : Color.green;
                Debug.DrawRay(transform.position, _direction * _rayDistance, rayColor);

                while (IsPathBlocked())
                {
                    Debug.Log("Path Blocked");
                    GetDestination();
                }

                var targetToAggro = CheckForAggro();
                if (targetToAggro != null)
                {
                    _target = targetToAggro.GetComponent<AIMovement>();
                    _targetGameobject = targetToAggro.gameObject;
                    _currentState = DroneState.Chase;
                }
                
                break;
            }
            
            //Run away when there are 2 or more enemies nearby
            case DroneState.Running:
            {
                
                StartCoroutine(StartWander());

                break;
            }
            
            //Chase the target
            case DroneState.Chase:
            {
                //if the target is destroyed start wander
                if (_targetGameobject == null)
                {
                    _currentState = DroneState.Wander;
                    return;
                }
                
                transform.LookAt(_target.transform);
                transform.Translate(Vector3.forward * Time.deltaTime * 5f);
                
                //if the character isn´t knight the launcher look to the enemy
                if (gameObject.transform.tag != "Knight")
                {
                    Launcher.transform.LookAt(_target.transform);
                }

                //if the target is seen start atack
                if (Vector3.Distance(transform.position, _target.transform.position) < _attackRange)
                {
                    _currentState = DroneState.Attack;
                }
                
                //if the target run away start wander
                if (Vector3.Distance(transform.position, _target.transform.position) > _aggroRadius)
                {
                    _currentState = DroneState.Wander;
                }
                
                break;
            }
            
            //Attack/shoot the target
            case DroneState.Attack:
            {

                //if the enemy is destroyed wander another time
                if ( _targetGameobject == null)
                {
                    _currentState = DroneState.Wander;
                }
                //if the enemy run away chase another time
                else if (Vector3.Distance(transform.position, _target.transform.position) > _attackRange)
                {
                    _currentState = DroneState.Chase;
                }
                //if the character is a knight attack
                else if (_target != null && gameObject.tag == "Knight")
                {
                    AttackEnemy();
                }
                //if the character isn´t and knight shoot
                else if (_target != null)
                {
                    if (Time.time > attackNextFire)
                    {
                        attackNextFire = Time.time + attackRate;
                        Instantiate(ProjectilePrefab, Launcher.transform.position, Launcher.transform.rotation);
                    }
                }
                
                //Look at target
                transform.LookAt(_target.transform);
                
                break;
            }
        }
    }
    
    //Rotate the character, run away and start wander
    IEnumerator StartWander()
    {
        
        //Rotate
        _rotate = true;
        yield return new WaitForSeconds(0.001f);
        _rotate = false;
        
        //Run away
        transform.Translate(Vector3.forward * Time.deltaTime * 10f);
        
        yield return new WaitForSeconds(1f);
        
        _currentState = DroneState.Wander;
    }
    
    //Attack the enemy if is on range
    public void AttackEnemy()
    {
        if (Time.time > attackNextFire)
        {
            attackNextFire = Time.time + attackRate;
            Attack(_target.GetComponent<CharacterStats>());
        }
    }

    //Deal damage
    public void Attack(CharacterStats targetStats)
    {
        targetStats.TakeDamage(_damage);
    }
    
    //Run away from enemies
    public void StartToRun()
    {
        _currentState = DroneState.Running;
    }
    
    //Show the range on editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _aggroRadius);
    }

    //Check if the path is blocked
    private bool IsPathBlocked()
    {
        Ray ray = new Ray(transform.position, _direction);
        var hitSomething = Physics.RaycastAll(ray, _rayDistance, _layerMask);
        return hitSomething.Any();
    }

    //Get the direction where the character is heading
    private void GetDestination()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 4f)) + new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f, UnityEngine.Random.Range(-4.5f, 4.5f));

        _destination = new Vector3(testPosition.x, 1f, testPosition.z);

        _direction = Vector3.Normalize(_destination - transform.position);
        _direction = new Vector3(_direction.x, 0f, _direction.z);
        _desiredRotation = Quaternion.LookRotation(_direction);
    }

    //Check if there is a destination, if there isn't set one
    private bool NeedsDestination()
    {
        if (_destination == Vector3.zero)
            return true;

        var distance = Vector3.Distance(transform.position, _destination);
        if (distance <= _stoppingDistance)
        {
            return true;
        }

        return false;
    }
    
    
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);
    
    //Search enemies nearby
    private Transform CheckForAggro()
    {

        RaycastHit hit;
        var angle = transform.rotation * startingAngle;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for(var i = 0; i < 24; i++)
        {
            if(Physics.Raycast(pos, direction, out hit, _aggroRadius))
            {
                var drone = hit.collider.GetComponent<AIMovement>();
                if(drone != null && drone.Team != gameObject.GetComponent<AIMovement>().Team)
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.red);
                    return drone.transform;
                }
                else
                {
                    Debug.DrawRay(pos, direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(pos, direction * _aggroRadius, Color.white);
            }
            direction = stepAngle * direction;
        }

        return null;
    }
}

//Set the teams
public enum Team
{
    Red,
    Blue
}

//Set the states of the characters
public enum DroneState
{
    Wander,
    Chase,
    Attack,
    Running
}