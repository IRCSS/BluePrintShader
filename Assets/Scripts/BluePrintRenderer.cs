using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BluePrintRenderer : MonoBehaviour {

    // PARAMETERS ---------------------------
//  ------- --------------- -------------------
    public  GameObject      objectToRender;
    public  int             numberOfLayers;

    private CommandBuffer   cbPeeling;
    public  RenderTexture[] peelingIntermidate;
    private Camera          mainCam;
    private Material        depthPeelingMat;
    private Texture         whiteTexture;
    private Mesh            meshToDraw;
    private Matrix4x4       meshMVP;
    private int             captureSet;
//  ------- --------------- -------------------

	void Start () {
		
        if(objectToRender == null)
        {
            Debug.LogError("No object set for blueprint effect on: " + this.gameObject.name);
            this.enabled = false;
            return;
        }

        // ------------------------------------------------------------------

                             mainCam = Camera.main;
        if (mainCam == null) mainCam = GameObject.FindObjectOfType<Camera>();
        if (mainCam == null) Debug.LogError("No Camera found in the scene ");

        // ------------------------------------------------------------------

        peelingIntermidate = new RenderTexture[numberOfLayers];
        for(int i = 0; i < peelingIntermidate.Length; i++)
            peelingIntermidate[i] = new RenderTexture(mainCam.pixelWidth, mainCam.pixelHeight, 16, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                anisoLevel = 0,
                useMipMap  = false,
            };
        // ------------------------------------------------------------------
        depthPeelingMat = new Material( Shader.Find("Unlit/DepthPeeler"));
        if (depthPeelingMat == null) Debug.LogError("Could not find the shade Unlit/DepthPeeler");
        // ------------------------------------------------------------------
        cbPeeling      = new CommandBuffer();
        cbPeeling.name = "DepthPeeling";

        whiteTexture   = Texture2D.blackTexture;
        meshToDraw     = objectToRender.GetComponent<MeshFilter>().sharedMesh;


        for (int i = 0; i<numberOfLayers; i++)
        {
            if(i == 0) cbPeeling.SetGlobalTexture("_PreviusLayer", whiteTexture);
            else       cbPeeling.SetGlobalTexture("_PreviusLayer", peelingIntermidate[i-1]);
            cbPeeling.SetRenderTarget(peelingIntermidate[i]);
            cbPeeling.ClearRenderTarget(true, true, Color.white);

            meshMVP = objectToRender.transform.localToWorldMatrix;
          
            cbPeeling.DrawMesh(meshToDraw, meshMVP, depthPeelingMat, 0, 0);
        }

        //cbPeeling.Blit(peelingIntermidate[1], BuiltinRenderTextureType.CameraTarget);
        mainCam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, cbPeeling);


	}
	
	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.F))
        {
            captureSet++;
            int i = 0;
            foreach(RenderTexture t in peelingIntermidate)
            {
                
                SaveRenderTexture.Save(t, SaveRenderTexture.OutPutType.JPEG, objectToRender.name+"Set" + captureSet + "Layer"+i);
                i++;
            }
        }
    }
}
