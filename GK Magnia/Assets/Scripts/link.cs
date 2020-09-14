using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.CodeDom.Compiler;
using UnityEngine.UI;
 
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshCollider))]
public class link : MonoBehaviour
{

    [SerializeField] public List<link> links;
    public List<link> GluedInGameNodes;
    private float isgravtime;
    [HideInInspector] private GameObject[] Aims; // созданные новые объекты целей из присоединенных кусков
    public GameObject[] Targets; // храним здесь непосредственно ноды для вычисления расстояний и углов для склейки
    [HideInInspector] private float[] targetdistances;
    [HideInInspector] public FixedJoint[] joints;
    public Vector3 RandomRotateOnStart;
    public Vector3 midpoint = Vector3.zero;
    [HideInInspector] private float[] targetangles;
    [HideInInspector] public static Vector2 Arot, Zrot, Across;
    [HideInInspector] public bool move, rotate, rotatey,twohand;
    [HideInInspector] public float sign, movespeed, rotatespeed;
    [HideInInspector] public Vector3 TransformPower, touchpos;
    [HideInInspector] private Vector3 a, b;
    private float maxVel, particleEmitDelay;
    public float RotateyPower;
    [HideInInspector] private Button Moverator;
    [HideInInspector] private float mass;
    [HideInInspector] private float[] GlueTime;
    [HideInInspector] public link[] LinksToGlue;
    [HideInInspector] private GameObject myCur;
    [HideInInspector] private GameObject MoveHitHelper;
    [HideInInspector] public Vector3 LocalStartPos, WorldStartPos,LastPos,LastVelocity;
    [HideInInspector] public Quaternion LocalStartRot, WorldStartRot;
    [HideInInspector] private float idle;
    private List<Vector3> lastAngularVelocities = new List<Vector3>();
    private int lastAngularVelositiesCount = 1;

    [HideInInspector] private float MouseOver;
    [HideInInspector] public FixedJoint FJ;
    [HideInInspector] public Rigidbody RB;
    private Vector3 tempVel;

    [HideInInspector] public ParticleSystem SmokeEffect,GlueEffect;
    [HideInInspector] private GameObject Dust; //Assign the Particle from the Editor (You can do this from code too)
    [HideInInspector] public Mesh MeshForParticleMagicEffect;
    [HideInInspector] public MeshCollider MC;
    private Vector3 StartRotationOTheMoment;
    private int LMB = 0, RMB = 1;
    public float startDrag,startmass,startAngular;
    private GameObject parentCollisionPartSys;
    private Transform transformParentCollisionPartSys;
    private GameObject collisionPartSysGO;
    private ParticleSystem collisionPartSys;
    private Transform collisionPartSysTransform;
    public Transform linkTransform;
    public List<Vector3> startCollisionPointPos;
    private ContactPoint[] collisionContactPoints;
    private Vector3 summPosContactPoints;
    private float temp;
    private Vector3 NewMidpoint;
    private Vector3 Stabilization;
    [HideInInspector] [SerializeField] public MeshRenderer mat;
    [HideInInspector] [SerializeField] public Material matLocked;
    [HideInInspector] [SerializeField] public Material matStart;
    private float gravhelper;
    private float dis;
    private Vector3 LastRotationEuler;

    public Vector3 LastAngularVelocity
    {
        get
        {
            Vector3 result = new Vector3();
            for (int i = 0; i < lastAngularVelocities.Count; i++)
            {
                result += lastAngularVelocities[i];
            }
            return result / lastAngularVelocities.Count;
        }
        set
        {
            lastAngularVelocities.Add(value);
            if (lastAngularVelocities.Count > lastAngularVelositiesCount)
            {
                lastAngularVelocities.RemoveAt(0);
            }
        }
    }

    // Не забыть удалить попозже
    void OnValidate()
    {
        if (mat == null) mat = GetComponent<MeshRenderer>() ;
    }
    private void Reset()
    {
        RB = GetComponent<Rigidbody>();
        MC = GetComponent<MeshCollider>();
     if(MC.convex==false)   MC.convex = true;
        if (mat == null) mat = GetComponent<MeshRenderer>() ;
    }

    virtual public void InitializeNode()
    {
          if(mat==null) mat = GetComponent<MeshRenderer>() ;
       
              //  matStart =new Material(GetComponent<MeshRenderer>().material);
      
      //  matLocked=new Material(mat.material);
     //   matLocked.name+= "_Locked";
     //   matLocked.SetColor("_EmissionColor", Color.black);
    //    matLocked.EnableKeyword("_EMISSION");

        particleEmitDelay = 0;
        GlueTime = new float[links.Count];
        twohand = false;
        Targets = new GameObject[links.Count];
        targetdistances = new float[links.Count];
        targetangles = new float[links.Count];
        joints = new FixedJoint[links.Count];
        LinksToGlue = new link[links.Count];
        GluedInGameNodes = new List<link>();

        LocalStartPos = transform.localPosition;
        LocalStartRot = transform.localRotation;
        WorldStartPos = transform.position;
        WorldStartRot = transform.rotation;

        for (int i = 0; i < links.Count; i++)
        {

            Targets[i] = new GameObject();
            Targets[i].transform.SetParent(transform);
            Targets[i].transform.localPosition = Vector3.zero;
            Targets[i].transform.localRotation = Quaternion.Euler(Vector3.zero);

            Targets[i].transform.SetParent(links[i].transform);
            Targets[i].transform.localScale = Vector3.one;
            Targets[i].name = gameObject.name + "-> " + links[i].name;

        }

        for (int i = 0; i < joints.Length; i++)
        {

            LinksToGlue[i] = Targets[i].transform.parent.gameObject.GetComponent<link>();
        }

        if (RB == null) RB = GetComponent<Rigidbody>();
        if (MC == null) MC = GetComponent<MeshCollider>();
        MC.isTrigger = false;
      if(  MC.convex==false) MC.convex = true;
        RB.isKinematic = true;
        GlueTime = new float[links.Count];
    
        
        startDrag = RB.drag;
        startmass = RB.mass;
        startAngular = RB.angularDrag;
       

        MouseOver = 0;
        idle = 0;

        movespeed = 1;
        rotatespeed = 3.25f;

        TransformPower = Vector2.zero;

        rotate = false;
        rotatey = false;
        move = false;

        isgravtime = 0;
        RB.useGravity = false;
        RB.drag = 5;
        RB.angularDrag = 10;

        CreateGlueEffectPrefab(null);
        CreateSmokeEffectPrefab(null);
      
    }


    void SaveVelocity()
    {
        if (move)
        {
            LastVelocity = RB.transform.position - LastPos;
            LastPos = RB.transform.position;
        }
    }
    public virtual void Update()
    {

        if (GameManager.CurrentState == GameManager.Mode.Play)
        {
            idle += Time.deltaTime;
            particleEmitDelay += Time.deltaTime;
            if (Input.GetMouseButtonUp(1))
            {
                rotate = false;
                rotatey = false;
                twohand = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                move = false;
                twohand = false;
            }


            // if (rotate == true) GameManager.IsRotate = true; else GameManager.IsRotate = false;



            if (this == GameManager.LockedObject)
            {
                SaveVelocity();
                //        Color c = Color.white;
                //     float emission = Mathf.PingPong(Time.time * 0.33f, 0.53f);
                //      mat.material.SetColor("_EmissionColor", emission * c);
            }

        }
        else
        {
         //    mat.material.DisableKeyword("_EMISSION");
         //    mat.material.SetColor("_EmissionColor", Color.black);
        }
    }
    public virtual void SelectNodeAndOutlineNewMeshes(link NewLinkLocked)
    {
        
        if (GameManager.LockedObject != null)
        {

            if (NewLinkLocked != GameManager.LockedObject)
            {
            
                //GameManager.LockedObject.mat.material.DisableKeyword("_EMISSION");             
                //GameManager.LockedObject.mat.material.SetColor("_EmissionColor", Color.black);
              
                //GameManager.LockedObject.UseGlueEffect(false);

            }

        }
        
        GameManager.LockedObject = NewLinkLocked;
      
        //mat.material.EnableKeyword("_EMISSION");
        
        idle = 0;

      //  if(NewLinkLocked.GetType()!=typeof(Spam))
        GameManager.LockedObject.gameObject.layer = GameManager.LayerGO;
       // GameManager.LockedObject.UseGlueEffect(true);
        
    }

    public virtual void FixedUpdate()
    {
        if (GameManager.LockedObject == null) SelectNodeAndOutlineNewMeshes(this);
        if (GameManager.CurrentState == GameManager.Mode.Play )
        {
            if (GameManager.LockedObject == this)
            {
                
               
                MoveRotateRotatey();// если поменять местами порядок вызова этих функций то лок объекта не будет работать

 
            }
            Inertia();
          
       
          
          if(GameManager.MyTimer>1.5f)    Glue3d();


            /*      
  if(RB.useGravity==true) { 
    if (RB.velocity.sqrMagnitude > 0.01f)
        isgravtime += Time.fixedDeltaTime;
    else isgravtime = 0;
if(move)     
    if (RB.velocity.y < 0) RB.velocity = new Vector3(RB.velocity.x, RB.velocity.y /1.4f, RB.velocity.z);
  } */
        }


    }



  

    public void ResetNodeWorldForTowerLevels()
    {
        RandomRotateOnStart = UnityEngine.Random.onUnitSphere;


        //   transform.position = WorldStartPos;
        //  transform.rotation = WorldStartRot;
        rotate = false;
        rotatey = false;
        move = false;
        twohand = false;
        for (int i = 0; i < joints.Length; i++) DestroyImmediate(joints[i]);

    }

    public void ResetNodeLocalForPuzzle()
    {
        twohand = false;
        RandomRotateOnStart = UnityEngine.Random.onUnitSphere;
        for (int i = 0; i < GlueTime.Length; i++) GlueTime[i] = 0;

        transform.localPosition = LocalStartPos;
        transform.localRotation = LocalStartRot;
        rotate = false;
        rotatey = false;
        move = false;
        GluedInGameNodes.Clear();
        for (int i = 0; i < joints.Length; i++) DestroyImmediate(joints[i]);

    }

    

  
 

    link FindNode(Transform t)
    {

        for (int i = 0; i < GameManager.AllNodes.Count; i++)
        {

            if (GameManager.AllNodes[i].transform == t) return GameManager.AllNodes[i];

        }
        return null;
    }
    void NewSelectionOfObjectsMobile()
    {
        link temp = null;
        RaycastHit hit;

     

        // if (this.GetType() == typeof(link) )
        //    RB.drag = Mathf.Lerp(RB.drag, startDrag, Time.fixedDeltaTime);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));


        if (Physics.Raycast(ray, out hit, 122, GameManager.LayerDefaultGOSpam)) temp = FindNode(hit.transform);

       



        if (Input.GetMouseButton(RMB) && Input.GetMouseButton(LMB))
        {
            if (this.GetType() == typeof(link))
                LockPosition(temp);

        }
        else if (Input.GetMouseButtonDown(LMB))
        {

            if (temp == null) // ничего не тыкнуто, активируем мув
            {

                   ActivateRotate();  

            }
            else if (temp == this)
            {

                ActivateMove();
                SelectNodeAndOutlineNewMeshes(temp);

            }
            else // тыкнут новый объект выбираем его и двигаем
            {

                SelectNodeAndOutlineNewMeshes(temp);
                temp.ActivateMove();

            }

          

        }
        else if (Input.GetMouseButtonDown(RMB))
        {


 

              //  if (hit.transform != GameManager.LockedObject.transform)
                ActivateRotatey();
 

            

        }

        if (Input.GetMouseButtonUp(RMB) && Input.GetMouseButton(LMB))
        {
            GameManager.LockedObject.ActivateMove();
        }
        else
        if (Input.GetMouseButtonUp(LMB) || Input.GetMouseButtonUp(RMB)) DisableAllActions();

    }
    protected virtual void NewSelectionOfObjects()
    {
        
        link temp = null;
        RaycastHit hit;
        
          //  if (GameManager.LevelGameDesign.UseGravity) RB.useGravity = true;

           // if (this.GetType() == typeof(link) )
            //    RB.drag = Mathf.Lerp(RB.drag, startDrag, Time.fixedDeltaTime);

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        

        if (Physics.Raycast(ray, out hit, 122, GameManager.LayerDefaultGOSpam)) temp = FindNode(hit.transform);

       



        if (Input.GetMouseButton(RMB) && Input.GetMouseButton(LMB))
        {
            if (this.GetType() == typeof(link))
                LockPosition(temp);

        }
        else if (Input.GetMouseButtonDown(LMB))
        {
                       
            if (temp == null) // ничего не тыкнуто, активируем мув
            {

                //  ActivateMove();  

            }
            else if (temp == this)
            {

                ActivateMove();
                  SelectNodeAndOutlineNewMeshes(temp);

            }
            else // тыкнут новый объект выбираем его и двигаем
            {

                SelectNodeAndOutlineNewMeshes(temp);
                temp.ActivateMove();

            }

       

        }
        else if (Input.GetMouseButtonDown(RMB))
        {

            

            if (temp == null)
            {
               
                if (hit.transform != GameManager.LockedObject.transform) ActivateRotatey();
                  
            }
            else if (temp == this && GameManager.LockedObject.GetType()==typeof(link))
            {

                ActivateRotate();
                SelectNodeAndOutlineNewMeshes(temp);

            }
            else
            if(temp.GetType() == typeof(link))
            {
                
                SelectNodeAndOutlineNewMeshes(temp);
                temp.ActivateRotate();

            }
            else
            {
                if (hit.transform != GameManager.LockedObject.transform) ActivateRotatey();
            }

            

        }

        if (Input.GetMouseButtonUp(RMB) && Input.GetMouseButton(LMB))
        {
            GameManager.LockedObject.ActivateMove();
        }
        else
        if (Input.GetMouseButtonUp(LMB)|| Input.GetMouseButtonUp(RMB)) DisableAllActions();
        
    }

    private void LockPosition( link temp)
    {
         
        RB.angularDrag = 10;
        RB.velocity *= 0.0f;
        

        twohand = true;
        RB.useGravity = false;
        RB.drag = 5;

        if (!rotate && !rotatey)
        {

            if (temp == this)
            {
                move = false;
                rotatey = false;
                rotate = true;

            }
            else
            {
                move = false;
                rotate = false;
                rotatey = true;

            }

        }

        touchpos = Input.mousePosition;
      
    }

   virtual protected void DisableAllActions()
    {

        if(move) RB.AddForce(LastVelocity*40,ForceMode.VelocityChange);
        if (rotate)
        {
            RB.angularVelocity = (LastAngularVelocity * Mathf.Deg2Rad);
        }
        if (rotatey) {
            RB.angularVelocity = (LastAngularVelocity * Mathf.Deg2Rad / 10);
        }
        GameManager.TransformingNow = false;
        RB.drag = startDrag;
        RB.angularDrag = startAngular;
        rotate = false;
        rotatey = false;
      //  RB.isKinematic = false;
        move = false;
        twohand = false;
       
     
       
    }

    void Inertia()
    {
     //   if (!move && !rotate && !rotatey && !!twohand)
     //   {
     //       RB.AddTorque(StartRotationOTheMoment-transform.rotation.eulerAngles);
      //      StartRotationOTheMoment = Vector3.zero;
      //  }
    }
    public void MoveNode(Vector3 Forc)
    {
        for (int i = 0; i < GluedInGameNodes.Count; i++)
        {

          //  GluedInGameNodes[i].RB.AddForce(Forc,ForceMode.VelocityChange);
            GluedInGameNodes[i].RB.transform.Translate(Forc * Time.fixedDeltaTime*5,Space.World);

        }
        //RB.AddForce(Forc, ForceMode.VelocityChange);
        RB.transform.Translate(Forc*Time.fixedDeltaTime*5, Space.World);
    }
    public virtual void RotateFunction()
    {
        if (rotate)
        {
            Arot = new Vector2(touchpos.x, touchpos.y) - new Vector2(Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).x, Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).y);

            TransformPower = ((Arot / Screen.width * 20)).magnitude * (((touchpos - Input.mousePosition) / Screen.width) * 10);



            TransformPower *= 1.2f;
            temp = Mathf.Lerp(temp, temp, Time.fixedDeltaTime * 5);
            for (int i = 0; i < GluedInGameNodes.Count; i++)
            {

                GluedInGameNodes[i].transform.RotateAround(NewMidpoint, new Vector3(-TransformPower.y, TransformPower.x, 0), temp);
                //  RB.AddTorque(new Vector3(-TransformPower.y * 500, TransformPower.x * 100, 0) * temp * 100, ForceMode.Acceleration);
                GluedInGameNodes[i].RB.AddForce(Stabilization);
                //  GluedInGameNodes[i].RB.angularVelocity += new Vector3(3*-TransformPower.y, 3*TransformPower.x, 0);
            }

            //  transform.rotation =Quaternion.Lerp(transform.rotation,Quaternion.Euler(StartRotationOTheMoment.eulerAngles.x, transform.rotation.eulerAngles.y, StartRotationOTheMoment.eulerAngles.z ),Time.fixedDeltaTime*5);
            RB.AddForce(Stabilization);
            transform.RotateAround(NewMidpoint, new Vector3(-TransformPower.y, TransformPower.x, 0), temp);
            LastAngularVelocity = new Vector3(-TransformPower.y, TransformPower.x, 0) * temp;

            // RB.angularVelocity += new Vector3(-TransformPower.y, TransformPower.x, 0);
            //   RB.AddTorque(  new Vector3(-TransformPower.y * 500, TransformPower.x * 100, 0)* temp,ForceMode.Acceleration);   

            {

                // GameManager.LockedObject.RB.AddTorque(0, 6 * TransformPower / 80, 0);
                // GameManager.LockedObject.RB.AddTorque(-6 * rotY / 80, 0, 0);

            }

            touchpos = Input.mousePosition;

        }

    }
    public virtual void MoveFunction()
    {
        if (move)
        {
            GameManager.MouseTarget.SetActive(true);
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(ray, out hit, 122, GameManager.LayerMouseTarget))
            {
                {
                    /*
                    Arot = new Vector2(touchpos.x, touchpos.y);
                    Arot.x = Mathf.Clamp(Arot.x, 0, Screen.width);
                    Arot.y = Mathf.Clamp(Arot.y, 0, Screen.height);
                      Arot -= new Vector2(Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).x, Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).y);


                    gravhelper =20* RB.mass*((Arot / Screen.width * 13.33f)).magnitude ;
                    gravhelper = Mathf.Clamp(gravhelper, 0.5f, 20);
                    RB.AddForce(gravhelper *( (-new Vector3(transform.position.x, transform.position.y, 0) +new Vector3(hit.point.x, hit.point.y, 0))));
                    */
                     
                     gravhelper = 0;
                         for (int i = 0; i < GluedInGameNodes.Count; i++)
                         {

                             Vector3 Difference = -RB.transform.position + new Vector3(hit.point.x, hit.point.y, RB.transform.position.z*0.65f);
                             //  GluedInGameNodes[i].RB.AddForce(10 * (new Vector3(hit.point.x, hit.point.y, 0) - RB.transform.position) / Screen.width, ForceMode.VelocityChange);
        GluedInGameNodes[i].RB.transform.position = Vector3.Lerp(GluedInGameNodes[i].RB.transform.position, GluedInGameNodes[i].RB.transform.position+Difference, 7 * Time.fixedDeltaTime);
                         }
                        //   else

                          
                        RB.transform.position = Vector3.Lerp(RB.transform.position, new Vector3(hit.point.x, hit.point.y, RB.transform.position.z * 0.65f), 7 * Time.fixedDeltaTime);
                    touchpos = Input.mousePosition;
                }

                //   RB.AddForce((GameManager.LevelGameDesign.isGravMult*Vector3.down*isgravtime * isgravtime));

            }
        }
    }

    public virtual void RotateyFunction()
    {
        if (rotatey)
        {
            touchpos = Input.mousePosition;

            Vector3 Across = Vector3.Cross(Vector3.back, Zrot);

            Arot = new Vector2(touchpos.x, touchpos.y) - new Vector2(Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).x, Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).y);

            dis = Arot.magnitude / Screen.width;
            //  dis *= dis * dis ;
            dis = 1;
            RotateyPower = Vector3.Angle(Zrot, Arot) * RB.mass;
            // RotateyPower *= 0.5f + GameManager.GameParams.RotatePower * rotatespeed * dis;

            sign = (Vector3.Dot(Arot, Across));
            //  RB.maxAngularVelocity = 100;
            // GameManager.LockedObject.transform.Rotate(-Mathf.Sign(sign) * Vector3.forward * 75 * RotateyPower * Time.fixedDeltaTime, Space.World);
            tempVel = 0.1f * RotateyPower * -Mathf.Sign(sign) * Vector3.forward;

            for (int i = 0; i < GluedInGameNodes.Count; i++)
            {

                //       GluedInGameNodes[i].RB.angularVelocity += (GluedInGameNodes.Count) * tempVel;
                GluedInGameNodes[i].transform.RotateAround(NewMidpoint, -Mathf.Sign(sign) * Vector3.forward * 100 * RotateyPower, temp);
                  GluedInGameNodes[i].RB.AddForce(Stabilization);
            }

                 RB.AddForce(Stabilization);

            
            {
                RB.angularVelocity += tempVel * 9 * RotateyPower;
            }
            //RB.angularVelocity += (GluedInGameNodes.Count + 1) * (GluedInGameNodes.Count+1)*(GluedInGameNodes.Count + 1) * (GluedInGameNodes.Count + 1) * 2 * RotateyPower *- Mathf.Sign(sign) * Vector3.forward;// * 15 * RotateyPower;

            LastAngularVelocity = -Mathf.Sign(sign) * Vector3.forward * 100 * RotateyPower * temp;

            touchpos = Input.mousePosition;

            Zrot = Arot;

        }
    }

    private void UnrotateFunction()
    {
        if (!rotate && !rotatey)
        {
            LastAngularVelocity = Vector3.zero;
        }
    }

    public   virtual void MoveRotateRotatey()
    {
        
        if (GameManager.CurrentState == GameManager.Mode.Play)
        {
              temp = Vector2.Distance(touchpos, new Vector2(Input.mousePosition.x, Input.mousePosition.y))/(Screen.width * 0.002f);

             NewMidpoint = Vector3.zero;

            for (int i = 0; i < GluedInGameNodes.Count; i++) NewMidpoint += GluedInGameNodes[i].transform.position;

            NewMidpoint += transform.position;
            NewMidpoint /= GluedInGameNodes.Count + 1;

            Stabilization = midpoint - NewMidpoint;

            if (move||rotate||rotatey)
            {

              
                //if (temp > 0.000001f) короче если это включить то объекты перестанут двигаться за мышой когда она статична
                {

                    if (GameManager.LockedObject == this)
                    {
                      //  MoveFunction();
                     //   RotateFunction();
                       // RotateyFunction();
                        //UnrotateFunction();
                    }

                }

            }               

            midpoint = NewMidpoint;
        }
    }

    void ActivateRotatey()
    {
        
        GameManager.TransformingNow = true;

        GameManager.LockedObject.RB.angularVelocity *= 0.01f;
        rotate = false;
        move = false;
        Arot = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
       Zrot = Arot - new Vector2(Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).x, Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).y);
         
        TransformPower = Vector2.zero;
        rotatey = true;
        touchpos = Input.mousePosition;
        idle = 0;
        Vector3 Across = Vector3.Cross(Vector3.back, Zrot);
        Arot =new Vector2 ( touchpos.x,touchpos.y) - new Vector2(Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).x, Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position).y);
        Vector2 bb = (Camera.main.WorldToScreenPoint(GameManager.LockedObject.transform.position));
        RB.angularDrag = 10;
    }

    void ActivateRotate()
    {

        // GameManager.LockedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // LockedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
        // LockedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY;

        StartRotationOTheMoment = transform.rotation.eulerAngles;
        GameManager.LockedObject.RB.angularVelocity *= 0.1f;
        move = false;
        rotate = true;
        rotatey = false;
        GameManager.TransformingNow = true;
        idle = 0;
        TransformPower = Vector2.zero;
        RB.angularDrag = 10;
        touchpos = Input.mousePosition;
        // LockedObject.GetComponent<Rigidbody>().isKinematic = true;// чтобы при клике на объект он сначала фризился, 
        // LockedObject.GetComponent<Rigidbody>().isKinematic = false;// для остановки вращения или смещения

    }

    void ActivateMove()
    {
          
        idle = 0;
        GameManager.LockedObject.RB.velocity *= 0.01f;
        GameManager.LockedObject.RB.angularVelocity *= 0.001f;

        RB.drag = 5;
        RB.angularDrag = 10;
        GameManager.TransformingNow = true;
        move = true;
        rotatey = false;
        rotate = false;
        touchpos = Input.mousePosition;
        twohand = false;

    }

    private  void FindNearesLink()
    {
        float min = 1000;
        link templink = null;
        for (int i = 0; i < links.Count; i++)
            if(links[i]!=null)
        {
            float temp = Vector3.Distance(links[i].transform.position, transform.position);

            if (min > temp)
            {
                min = temp;
                templink = links[i];
            }
        }

        

       
    }
    virtual protected void Glue3d()
    {

      if(links.Count>0)  GameManager.CheckWinGame();
        for (int i = 0; i < links.Count; i++)
        {
            
            if (joints[i] == null)
            {
                
                a = new Vector3(transform.position.x, transform.position.y, 0);
                b = new Vector3(Targets[i].transform.position.x, Targets[i].transform.position.y, 0);

                targetdistances[i] = 100 *   Vector3.SqrMagnitude(a - b);
                targetangles[i] = Quaternion.Angle(transform.rotation, Targets[i].transform.rotation);


                FindNearesLink();

                float DifficultyGlueParam =2* (4-GameSequence.instance.CurrentDifficulty)+0.1f ;

                if ((targetdistances[i] < 20 ))
                {
                    //Vector3 relativePos = Targets[i].transform.position - transform.position;

                    //links[i].transform.rotation = Quaternion.Slerp(links[i].transform.rotation, Targets[i].transform.rotation, GlueTime[i] * GlueTime[i]);
                    Targets[i].transform.parent.rotation = Quaternion.RotateTowards(Targets[i].transform.parent.rotation, transform.rotation, 50 *Time.fixedDeltaTime);
                }

                if ((targetdistances[i] <1))
                {

                    if (GlueTime[i] > 0.7f)
                    {
                        MC.isTrigger = true;
                        //  MC.isTrigger = true;
                        //  RB.isKinematic = true;


                    }
                    GlueTime[i] += Time.fixedDeltaTime;
                    
                    transform.position = Vector3.Lerp(transform.position, Targets[i].transform.position, GlueTime[i] *  GlueTime[i]   );
                    transform.rotation = Quaternion.Lerp(transform.rotation, Targets[i].transform.rotation, GlueTime[i] * GlueTime[i]  );

                    UseSmokeEffect(true);
                    
                    if (GlueTime[i] > 0.88f)
                    {
                        
                        GameManager.WinJoint++;

                        transform.position = Targets[i].transform.position;
                        transform.rotation = Targets[i].transform.rotation;

                      

                         joints[i] = (FixedJoint)gameObject.AddComponent(typeof(FixedJoint));
                         joints[i].connectedBody = LinksToGlue[i].RB;
                       
                        GluedInGameNodes.Add(LinksToGlue[i]);
                        // links[i] = null;

                        //if поставлен для правки бага, когда мы перемещаем \ вертим какой-то объект, а совершенно другие пару объектов склеиваются. 
                        //Без этой правки, дивижение или вращение выделенного объекта останавливается, что мешает очень играть.
                        if (GameManager.LockedObject == this | GameManager.LockedObject == Targets[i]) SelectNodeAndOutlineNewMeshes(this); 

                       
                        MC.isTrigger = false;
                        //  RB.isKinematic = false;
                        //  Debug.Log("XUY3");

                    
                    }

                }
                else
                {

                   
                    GlueTime[i] = 0;
                    MC.isTrigger = false;
                }

           

                  

        }
        }
    }

   
     
    public virtual void OnCollisionEnter(Collision collision)
    {        

        if (particleEmitDelay > 1.5f && GameManager.CurrentState == GameManager.Mode.Play)
        {

            maxVel = -100;

            for (int i = 0; i < collision.contacts.Length; i++)
            {

                if (maxVel < collision.contacts[i].point.magnitude) maxVel = collision.contacts[i].point.magnitude;

            }

           // if (maxVel > GameManager.LevelGameDesign.DustEffectEmitTreshhold)
            {

                particleEmitDelay = 0;
                
            }

            /// <summary>
            /// Проверка на столкновения с кусками собираемого объекта для ачивок 19 и 20. 
            /// Если столкнулись, то бул в тру, на которую после выигрыша и будем смотреть.
            /// </summary>
          
        }

    }

    
    public void  UseGlueEffect(bool state)
    {

        if (GlueEffect != null)
        {

            if (state) GlueEffect.gameObject.SetActive(true);
            else GlueEffect.gameObject.SetActive(false);

        }

    }

    public void UseSmokeEffect(bool state)
    {
        if (GlueEffect != null)
        {
            if (state)
            {
                SmokeEffect.gameObject.SetActive(true);
            }
            else
            {
                SmokeEffect.gameObject.SetActive(false);
            }
        }
        
    }

        public void CreateGlueEffectPrefab(GameObject PS)
    {

        if (GlueEffect == null)
        {
            if (PS == null) PS = Resources.Load<GameObject>("VFX/vfx_GlueIt");
           
            GameObject GO = Instantiate(PS);
            GO.transform.SetParent(transform);
            GO.transform.localPosition = Vector3.zero;
            GO.transform.localRotation = Quaternion.Euler(Vector3.zero);
           
            GlueEffect = GO.GetComponent<ParticleSystem>();
            GO.transform.localScale = Vector3.one;
            var MS = GlueEffect.shape;

            MS.mesh = GetComponent<MeshCollider>().sharedMesh;
            if (MS.mesh != null) MS.scale = gameObject.transform.parent.localScale;
            GlueEffect.gameObject.SetActive(false);
            GlueEffect.Stop();           
           
        }
    }

    public void CreateSmokeEffectPrefab(GameObject PS)
    {

        if (SmokeEffect == null)
        {
            if (PS == null) PS = Resources.Load<GameObject>("VFX/vfx_smoke");
            GameObject GO = Instantiate(PS);
            GO.transform.SetParent(transform);
            GO.transform.localPosition = Vector3.zero;
            GO.transform.localRotation = Quaternion.Euler(Vector3.zero);

            SmokeEffect = GO.GetComponent<ParticleSystem>();
            GO.transform.localScale = Vector3.one;
            var MS = SmokeEffect.shape;

            MS.mesh = GetComponent<MeshCollider>().sharedMesh;
            if (MS.mesh != null) MS.scale = gameObject.transform.parent.localScale;
            SmokeEffect.Stop();
            SmokeEffect.gameObject.SetActive(false);

        }
    }



}

