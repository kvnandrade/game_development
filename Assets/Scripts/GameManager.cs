using System.Collections;
using UnityEngine;

namespace ValeDosCristais
{
    public sealed class GameManager : MonoBehaviour
    {
        private GameObject levelRoot;
        private PlayerController player;
        private AudioManager audioManager;
        private HudController hud;
        private CameraFollow cameraFollow;
        private Vector2 currentSpawn;
        private int currentLevelIndex;
        private int lives = 3;
        private int score;
        private int levelCollectibles;
        private int collectedThisLevel;
        private bool transitioning;

        private void Start()
        {
            Application.targetFrameRate = 60;
            audioManager = gameObject.AddComponent<AudioManager>();
            hud = HudController.Create(transform);
            ConfigureCamera();
            LoadLevel(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                LoadLevel(currentLevelIndex);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void Collect(Collectible collectible)
        {
            if (collectible.Type == CollectibleType.Life)
            {
                lives++;
                score += collectible.ScoreValue;
                hud.ShowMessage("Vida extra!");
                audioManager.Play("life");
                SpawnBurst(collectible.transform.position, new Color(1f, 0.25f, 0.35f));
            }
            else
            {
                score += collectible.ScoreValue;
                hud.ShowMessage("+" + collectible.ScoreValue + " pontos");
                audioManager.Play("collect");
                SpawnBurst(collectible.transform.position, collectible.Type == CollectibleType.Gem ? Color.cyan : Color.yellow);
            }

            collectedThisLevel++;
            UpdateHud();
        }

        public void PlayerDamaged(PlayerController damagedPlayer)
        {
            if (transitioning)
            {
                return;
            }

            lives--;
            audioManager.Play("damage");
            SpawnBurst(damagedPlayer.transform.position, new Color(1f, 0.2f, 0.15f));

            if (lives <= 0)
            {
                lives = 3;
                score = Mathf.Max(0, score - 100);
                hud.ShowMessage("Fim de jogo! Reiniciando fase...");
                UpdateHud();
                StartCoroutine(ReloadLevelAfterDelay());
            }
            else
            {
                hud.ShowMessage("Dano sofrido! Vidas restantes: " + lives);
                UpdateHud();
                damagedPlayer.Respawn();
            }
        }

        public void CompleteLevel()
        {
            if (transitioning)
            {
                return;
            }

            transitioning = true;
            score += 100 + collectedThisLevel * 5;
            audioManager.Play("goal");
            SpawnBurst(player.transform.position + Vector3.up, Color.cyan);

            if (currentLevelIndex >= LevelData.All.Count - 1)
            {
                hud.ShowMessage("Cristal-Mor restaurado! Voce venceu!");
                UpdateHud();
                StartCoroutine(RestartCampaignAfterDelay());
            }
            else
            {
                hud.ShowMessage("Fase concluida!");
                UpdateHud();
                StartCoroutine(NextLevelAfterDelay());
            }
        }

        private void LoadLevel(int levelIndex)
        {
            transitioning = false;
            currentLevelIndex = Mathf.Clamp(levelIndex, 0, LevelData.All.Count - 1);
            LevelData data = LevelData.All[currentLevelIndex];

            if (levelRoot != null)
            {
                Destroy(levelRoot);
            }

            levelRoot = new GameObject("Level - " + data.Name);
            levelRoot.transform.SetParent(transform, false);
            levelCollectibles = 0;
            collectedThisLevel = 0;
            currentSpawn = new Vector2(2f, 3f);

            CreateBackground(data);
            BuildLevel(data);
            EnsurePlayer();
            player.SetSpawn(currentSpawn);

            if (cameraFollow != null)
            {
                cameraFollow.Target = player.transform;
                cameraFollow.LevelBounds = new Vector2(data.Width, data.Height + 2f);
            }

            hud.ShowMessage(data.Tip);
            UpdateHud();
        }

        private void BuildLevel(LevelData data)
        {
            for (int rowIndex = 0; rowIndex < data.Rows.Length; rowIndex++)
            {
                string row = data.Rows[rowIndex];
                float y = data.Rows.Length - 1 - rowIndex;
                for (int x = 0; x < row.Length; x++)
                {
                    char tile = row[x];
                    Vector2 position = new Vector2(x + 0.5f, y + 0.5f);
                    switch (tile)
                    {
                        case '#':
                            CreateSolidTile("Chao", "tile_ground", position);
                            break;
                        case '=':
                            CreateSolidTile("Plataforma", "tile_platform", position);
                            break;
                        case 'H':
                            CreateLadder(position);
                            break;
                        case 'C':
                            CreateCollectible(position, CollectibleType.Coin, 10, "coin_0");
                            break;
                        case 'G':
                            CreateCollectible(position, CollectibleType.Gem, 50, "gem_0");
                            break;
                        case '+':
                            CreateCollectible(position, CollectibleType.Life, 25, "heart_0");
                            break;
                        case 'E':
                            CreateEnemy(position);
                            break;
                        case '^':
                            CreateHazard(position);
                            break;
                        case 'X':
                            CreateGoal(position);
                            break;
                        case 'P':
                            currentSpawn = position + Vector2.up * 0.2f;
                            break;
                    }
                }
            }
        }

        private void EnsurePlayer()
        {
            if (player != null)
            {
                player.gameObject.SetActive(true);
                return;
            }

            GameObject playerObject = CreateSpriteObject("Player", "player_idle_0", currentSpawn, null, 10);
            playerObject.tag = "Player";
            Rigidbody2D body = playerObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 2.4f;
            BoxCollider2D box = playerObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.72f, 1.35f);
            box.offset = new Vector2(0f, -0.1f);
            playerObject.AddComponent<SpriteAnimator>();
            player = playerObject.AddComponent<PlayerController>();
            player.Configure(this, audioManager);
        }

        private void ConfigureCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                mainCamera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5.5f;
            mainCamera.backgroundColor = new Color(0.07f, 0.10f, 0.18f);
            cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow == null)
            {
                cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            }
        }

        private void CreateBackground(LevelData data)
        {
            GameObject sky = CreateSpriteObject("Ceu", "background_sky", new Vector2(data.Width * 0.5f, data.Height * 0.5f), levelRoot.transform, -10);
            sky.transform.localScale = new Vector3(data.Width, data.Height + 6f, 1f);

            for (int i = 0; i < data.Width; i += 8)
            {
                GameObject crystal = CreateSpriteObject("Cristal de fundo", "background_crystal", new Vector2(i + 2f, 1.2f + (i % 3)), levelRoot.transform, -4);
                crystal.transform.localScale = Vector3.one * (1.2f + (i % 4) * 0.15f);
            }
        }

        private void CreateSolidTile(string name, string spriteName, Vector2 position)
        {
            GameObject tile = CreateSpriteObject(name, spriteName, position, levelRoot.transform, 0);
            BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;
        }

        private void CreateLadder(Vector2 position)
        {
            GameObject ladder = CreateSpriteObject("Escada", "ladder", position, levelRoot.transform, 1);
            ladder.tag = "Ladder";
            BoxCollider2D collider = ladder.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.8f, 1f);
            ladder.AddComponent<Ladder>();
        }

        private void CreateCollectible(Vector2 position, CollectibleType type, int scoreValue, string spriteName)
        {
            GameObject item = CreateSpriteObject(type.ToString(), spriteName, position, levelRoot.transform, 5);
            item.tag = "Collectible";
            CircleCollider2D collider = item.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.35f;
            Collectible collectible = item.AddComponent<Collectible>();
            collectible.Configure(this, type, scoreValue);
            SpriteAnimator animator = item.AddComponent<SpriteAnimator>();
            if (type == CollectibleType.Coin)
            {
                animator.Define("spin", "coin_0", "coin_1");
            }
            else if (type == CollectibleType.Gem)
            {
                animator.Define("spin", "gem_0", "gem_1");
            }
            else
            {
                animator.Define("spin", "heart_0", "heart_1");
            }
            animator.Play("spin");
            levelCollectibles++;
        }

        private void CreateEnemy(Vector2 position)
        {
            GameObject enemy = CreateSpriteObject("Morcego de cristal", "enemy_walk_0", position + Vector2.up * 0.2f, levelRoot.transform, 6);
            enemy.tag = "Enemy";
            BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.9f, 0.75f);
            enemy.AddComponent<SpriteAnimator>();
            enemy.AddComponent<EnemyPatrol>();
        }

        private void CreateHazard(Vector2 position)
        {
            GameObject spike = CreateSpriteObject("Espinhos", "spike", position, levelRoot.transform, 2);
            spike.tag = "Hazard";
            BoxCollider2D collider = spike.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.85f, 0.65f);
            collider.offset = new Vector2(0f, -0.17f);
            spike.AddComponent<Hazard>();
        }

        private void CreateGoal(Vector2 position)
        {
            GameObject goal = CreateSpriteObject("Portal", "goal_0", position + Vector2.up * 0.5f, levelRoot.transform, 4);
            goal.tag = "Goal";
            BoxCollider2D collider = goal.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1f, 1.8f);
            goal.AddComponent<SpriteAnimator>();
            GoalPortal portal = goal.AddComponent<GoalPortal>();
            portal.Configure(this);
        }

        private GameObject CreateSpriteObject(string name, string spriteName, Vector2 position, Transform parent, int sortingOrder)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = position;
            if (parent != null)
            {
                gameObject.transform.SetParent(parent, true);
            }

            SpriteRenderer renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteLibrary.Get(spriteName);
            renderer.sortingOrder = sortingOrder;
            return gameObject;
        }

        private void SpawnBurst(Vector2 position, Color color)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2f / 8f;
                Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(1.4f, 3.2f);
                GameObject particle = CreateSpriteObject("Particula", "particle", position, null, 20);
                FadingParticle fading = particle.AddComponent<FadingParticle>();
                fading.Initialize(color, velocity, 0.65f);
            }
        }

        private void UpdateHud()
        {
            hud.SetStats(lives, score, collectedThisLevel, levelCollectibles, LevelData.All[currentLevelIndex].Name);
        }

        private IEnumerator ReloadLevelAfterDelay()
        {
            transitioning = true;
            yield return new WaitForSeconds(1.5f);
            LoadLevel(currentLevelIndex);
        }

        private IEnumerator NextLevelAfterDelay()
        {
            yield return new WaitForSeconds(1.5f);
            LoadLevel(currentLevelIndex + 1);
        }

        private IEnumerator RestartCampaignAfterDelay()
        {
            yield return new WaitForSeconds(4f);
            score = 0;
            lives = 3;
            LoadLevel(0);
        }
    }
}
