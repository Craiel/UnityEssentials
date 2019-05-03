namespace Craiel.UnityEssentials.Runtime.Utils
{
    using UnityEngine;
    
    public class TerrainSplitter : MonoBehaviour
    {
        private const int TilesPerSplit = 4;
        private static readonly int SplitPiecesSquared = (int) System.Math.Sqrt((int) TilesPerSplit);

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Terrain Source;

        [SerializeField]
        public GameObject TargetParent;

        [SerializeField]
        public string Pattern = @"{0}_{1}";

        [SerializeField] 
        public int SplitCount = 1;

        public void Split()
        {
            if (this.Source == null)
            {
                return;
            }

            this.DoSplit(this.Source, 1);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static TerrainData CopyTerrainData(int segment, TerrainData source)
        {
            TerrainData result = new TerrainData
            {
                splatPrototypes = source.splatPrototypes,
                detailPrototypes = source.detailPrototypes,
                treePrototypes = source.treePrototypes,
                heightmapResolution = source.heightmapResolution / SplitPiecesSquared,
                alphamapResolution = source.alphamapResolution / SplitPiecesSquared,
                wavingGrassAmount = source.wavingGrassAmount,
                wavingGrassSpeed = source.wavingGrassSpeed,
                wavingGrassStrength = source.wavingGrassStrength,
                wavingGrassTint = source.wavingGrassTint,
                hideFlags = source.hideFlags
            };

            Vector3 tileSize = new Vector3(source.size.x / SplitPiecesSquared, source.size.y,source.size.z / SplitPiecesSquared);
            result.size = tileSize;

            // Height Map
            float[,] sourceHeightMap = source.GetHeights(0, 0, source.heightmapResolution, source.heightmapResolution);
            float[,] splitHeightMap = SplitHeightMap(segment, sourceHeightMap, source.heightmapResolution);
            result.SetHeights(0, 0, splitHeightMap);

            // Splat Map
            float[,,] sourceSplatMap = source.GetAlphamaps(0, 0, source.alphamapResolution, source.alphamapResolution);
            float[,,] splitSplatMap = SplitSplatMap(segment, sourceSplatMap, source.alphamapLayers, source.alphamapResolution);
            result.SetAlphamaps(0, 0, splitSplatMap);

            // Detail Map
            result.SetDetailResolution(source.detailResolution / SplitPiecesSquared, 8);
            for (int i = 0; i < source.detailPrototypes.Length; i++)
            {
                int[,] sourceDetailMap = source.GetDetailLayer(0, 0, source.detailResolution, source.detailResolution, i);
                int[,] splitDetailMap = SplitDetailMap(segment, sourceDetailMap, source.detailResolution);
                result.SetDetailLayer(0, 0, i, splitDetailMap);
            }
            
            return result;
        }
        
        private void DoSplit(Terrain source, int depth)
        {
            for (int segment = 0; segment < TilesPerSplit; segment++)
            {
                TerrainData data = CopyTerrainData(segment, source.terrainData);
                GameObject segmentObject = SetupGameObject(segment, data, source.GetPosition());
                
                segmentObject.name = string.Format(this.Pattern, source.gameObject.name, segment);

                // Setup the terrain
                Terrain terrain = segmentObject.GetComponent<Terrain>();
                terrain.terrainData = data;

                // Copy Properties
                terrain.basemapDistance = source.basemapDistance;
                terrain.castShadows = source.castShadows;
                terrain.detailObjectDensity = source.detailObjectDensity;
                terrain.detailObjectDistance = source.detailObjectDistance;
                terrain.heightmapMaximumLOD = source.heightmapMaximumLOD;
                terrain.heightmapPixelError = source.heightmapPixelError;
                terrain.treeBillboardDistance = source.treeBillboardDistance;
                terrain.treeCrossFadeLength = source.treeCrossFadeLength;
                terrain.treeDistance = source.treeDistance;
                terrain.treeMaximumFullLODCount = source.treeMaximumFullLODCount;
                terrain.materialType = source.materialType;
                terrain.materialTemplate = source.materialTemplate;
                terrain.legacyShininess = source.legacyShininess;
                terrain.reflectionProbeUsage = source.reflectionProbeUsage;
                
                if (this.TargetParent != null)
                {
                    segmentObject.transform.SetParent(this.TargetParent.transform);
                }

                // Tree Data
                for (int i = 0; i < source.terrainData.treeInstances.Length; i++)
                {
                    TreeInstance sourceTreeData = source.terrainData.treeInstances[i];
                    SplitTreeData(segment, sourceTreeData, terrain);
                }

                if (depth < this.SplitCount)
                {
                    this.DoSplit(terrain, depth + 1);
                    
#if UNITY_EDITOR
                    DestroyImmediate(segmentObject);
#else
                    Destroy(segmentObject);
#endif
                }
            }
        }

        private static GameObject SetupGameObject(int segment, TerrainData data, Vector3 parentPosition)
        {
            GameObject result = Terrain.CreateTerrainGameObject(data);
            Transform resultTransform = result.transform;
            Vector3 currentPosition = resultTransform.position;
            
            float xWShift = (segment % SplitPiecesSquared) * data.size.z;
            float zWShift = (segment / SplitPiecesSquared) * data.size.x;

            resultTransform.position = new Vector3(
                currentPosition.x + zWShift,
                currentPosition.y,
                currentPosition.z + xWShift);

            currentPosition = resultTransform.position;
            // Shift last position
            resultTransform.position = new Vector3(
                currentPosition.x + parentPosition.x,
                currentPosition.y + parentPosition.y,
                currentPosition.z + parentPosition.z
            );

            return result;
        }

        private static float[,] SplitHeightMap(int segment, float[,] source, int heightMapResolution)
        {
            int heightResolutionPerSegment = heightMapResolution / SplitPiecesSquared;
            
            float[,] result = new float[heightResolutionPerSegment + 1, heightResolutionPerSegment + 1];

            int startX = 0;
            int startY = 0;

            int endX = heightResolutionPerSegment + 1;
            int endY = heightResolutionPerSegment + 1;

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {

                    int xShift = 0;
                    int yShift = 0;

                    switch (segment)
                    {
                        case 1:
                        {
                            xShift = heightResolutionPerSegment;
                            break;
                        }
                        
                        case 2:
                        {
                            yShift = heightResolutionPerSegment;
                            break;
                        }
                        
                        case 3:
                        {
                            xShift = heightResolutionPerSegment;
                            yShift = heightResolutionPerSegment;
                            break;
                        }
                    }

                    float value = source[x + xShift, y + yShift];
                    result[x, y] = value;
                }
            }

            return result;
        }

        private static float[,,] SplitSplatMap(int segment, float[,,] source, int alphaLayers, int alphaMapResolution)
                 {
            int alphaResolutionPerSegment = alphaMapResolution / SplitPiecesSquared;
            
            float[,,] result = new float[alphaResolutionPerSegment, alphaResolutionPerSegment, alphaLayers];

            int startX = 0;
            int startY = 0;

            int endX = alphaResolutionPerSegment;
            int endY = alphaResolutionPerSegment;
            
            for (int s = 0; s < alphaLayers; s++)
            {
                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY; y < endY; y++)
                    {
                        int xShift = 0;
                        int yShift = 0;

                        switch (segment)
                        {
                            case 1:
                            {
                                xShift = alphaResolutionPerSegment;
                                break;
                            }
                            
                            case 2:
                            {
                                yShift = alphaResolutionPerSegment;
                                break;
                            }
                            
                            case 3:
                            {
                                xShift = alphaResolutionPerSegment;
                                yShift = alphaResolutionPerSegment;
                                break;
                            }
                        }

                        float value = source[x + xShift, y + yShift, s];
                        result[x, y, s] = value;
                    }
                }
            }

            return result;
        }

        private static int[,] SplitDetailMap(int segment, int[,] source, int detailResolution)
        {
            int[,] result = new int[detailResolution / SplitPiecesSquared, detailResolution / SplitPiecesSquared];

            int detailResolutionPerSegment = detailResolution / SplitPiecesSquared;

            int startX = 0;
            int startY = 0;

            int endX = detailResolutionPerSegment;
            int endY = detailResolutionPerSegment;
			
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    int xShift = 0;
                    int yShift = 0;

                    switch (segment)
                    {
                        case 1:
                        {
                            xShift = detailResolutionPerSegment;
                            break;
                        }

                        case 2:
                        {
                            yShift = detailResolutionPerSegment;
                            break;
                        }

                        case 3:
                        {
                            xShift = detailResolutionPerSegment;
                            yShift = detailResolutionPerSegment;
                            break;
                        }
                    }

                    int value = source[x + xShift, y + yShift];
                    result[x, y] = value;
                }
            }

            return result;
        }

        private static void SplitTreeData(int segment, TreeInstance source, Terrain target)
        {
            if (segment == 0 &&
                source.position.x > 0f && source.position.x < 0.5f &&
                source.position.z > 0f && source.position.z < 0.5f)
            {
                // Recalculate new tree position	
                source.position = new Vector3(source.position.x * 2f, source.position.y, source.position.z * 2f);

                // Add tree instance						
                target.AddTreeInstance(source);
            }

            if (segment == 1 &&
                source.position.x > 0.0f && source.position.x < 0.5f &&
                source.position.z >= 0.5f && source.position.z <= 1.0f)
            {
                // Recalculate new tree position	
                source.position = new Vector3((source.position.x) * 2f, source.position.y, (source.position.z - 0.5f) * 2f);

                // Add tree instance						
                target.AddTreeInstance(source);
            }

            if (segment == 2 &&
                source.position.x >= 0.5f && source.position.x <= 1.0f &&
                 source.position.z > 0.0f && source.position.z < 0.5f)
            {
                // Recalculate new tree position	
                source.position = new Vector3((source.position.x - 0.5f) * 2f, source.position.y, (source.position.z) * 2f);

                // Add tree instance						
                target.AddTreeInstance(source);
            }

            if (segment == 3 &&
                source.position.x >= 0.5f && source.position.x <= 1.0f &&
                 source.position.z >= 0.5f && source.position.z <= 1.0f)
            {
                // Recalculate new tree position	
                source.position = new Vector3((source.position.x - 0.5f) * 2f, source.position.y, (source.position.z - 0.5f) * 2f);

                // Add tree instance						
                target.AddTreeInstance(source);
            }
        }
    }
}