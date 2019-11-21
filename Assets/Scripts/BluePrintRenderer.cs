using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BluePrintRenderer : MonoBehaviour {

    // PARAMETERS ---------------------------
//  ------- --------------- -------------------
    public  GameObject      objectToRender;
    public  int             numberOfLayers;
    [Range(0f,1f)]
    public  float           objectOpacity = 0.4f;

    private CommandBuffer   cbPeeling;
    private CommandBuffer   cbEdgeMapping;
    private RenderTexture[] peelingIntermidate;
    private RenderTexture   edgeMap;
    private RenderTexture   forwardPass;
    private Camera          mainCam;
    private Material        depthPeelingMat;
    private Material        edgeConstructionMat;
    private Material        combineOnScreen;
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

        // ==================================================================
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

        // ==================================================================
        // ------------------------------------------------------------------

        edgeMap             = new RenderTexture(peelingIntermidate[0]);
        edgeConstructionMat = new Material(Shader.Find("Unlit/EdgeMapping"));
        forwardPass         = new RenderTexture(edgeMap);
        // ------------------------------------------------------------------

        cbEdgeMapping       = new CommandBuffer();
        cbEdgeMapping.name  = "EdgeMapping";



        for (int i = 0; i < numberOfLayers; i++)
        {
            cbEdgeMapping.SetRenderTarget(edgeMap);
            cbEdgeMapping.ClearRenderTarget(true, true, Color.black);

            cbEdgeMapping.Blit(peelingIntermidate[i], edgeMap, edgeConstructionMat, 0);
            cbEdgeMapping.Blit(edgeMap, peelingIntermidate[i]);
            
        }

        

        for (int i = 0; i < numberOfLayers; i++)
        {
          //  cbEdgeMapping.SetRenderTarget(edgeMap);
            cbEdgeMapping.Blit(peelingIntermidate[i], edgeMap, edgeConstructionMat, 1);

        }

        combineOnScreen = new Material(Shader.Find("Unlit/CombineOnScreen"));

        

        cbEdgeMapping.Blit(BuiltinRenderTextureType.CameraTarget, forwardPass);
        cbEdgeMapping.SetGlobalTexture("_ForwadPass", forwardPass);
        cbEdgeMapping.Blit(edgeMap, BuiltinRenderTextureType.CameraTarget, combineOnScreen);
        cbEdgeMapping.Blit(BuiltinRenderTextureType.CameraTarget, edgeMap);

        mainCam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, cbEdgeMapping);

    }

    // Update is called once per frame
    void Update () {


        Shader.SetGlobalFloat("_ForwardPassContribution", objectOpacity);

        if (Input.GetKeyDown(KeyCode.F))
        {
            captureSet++;
            int i = 0;
            foreach(RenderTexture t in peelingIntermidate)
            {
                
                SaveRenderTexture.Save(t, SaveRenderTexture.OutPutType.JPEG, objectToRender.name+"Set" + captureSet + "Layer"+i);
                i++;
            }

            SaveRenderTexture.Save(edgeMap, SaveRenderTexture.OutPutType.JPEG, objectToRender.name + "Set" + captureSet + "FinalEdge");
        }
    }
}
