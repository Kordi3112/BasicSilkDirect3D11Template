using BasicSilkDirect3D11Template.Source.Scenes;
using Engine.Core;
using Silk.NET.Core.Native;
using Silk.NET.Maths;

Console.WriteLine("Hello, World!");

using (var gameManager = new GameManager())
{

    gameManager.InitResources += GameManager_InitResources;
    gameManager.InitScenes += GameManager_InitScenes;

    gameManager.Run("Title", new Vector2D<int>(800,600));

}

void GameManager_InitScenes(object sender, EventArgs e)
{
    GameManager gameManager = (GameManager)sender;

    Console.WriteLine("Scenes");

    gameManager.SceneManager.SetHeadScene(new HeadScene1());

    gameManager.SceneManager.AddScene(new Scene1());


    gameManager.SceneManager.ApplyScene("Scene1");
}

void GameManager_InitResources(object sender, EventArgs e)
{
    Console.WriteLine("Res");
}