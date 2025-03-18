using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Emulator;

class Program
{

    public static IWindow window = null!;
    public static GL gl = null!;
    public static IInputContext input = null!;
    public static ImGuiController imgui = null!;

    public static event Action<double> Update = null!;

    static void Main(string[] args)
    { 
        WindowOptions winopt = WindowOptions.Default with
        {
            Size = new(800, 600),
            Title = "Emulator",
            //TransparentFramebuffer = true,
            WindowBorder = WindowBorder.Hidden,
            VSync = false
        };

        window = Window.Create(winopt);

        window.Load += OnLoad;
        window.Closing += OnClose;
        window.Update += OnUpdate;
        window.Render += OnRender;

        window.FramebufferResize += (s) => gl.Viewport(0, 0, (uint)s.X, (uint)s.Y);

        window.Run();
    }

    private static void OnLoad()
    {
        gl = window.CreateOpenGL();
        input = window.CreateInput();

        window.Center();
        window.WindowState = WindowState.Maximized;

        imgui = new(gl, window, input);

        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
        
        ImGui.LoadIniSettingsFromDisk("imgui.ini");

        gl.ClearColor(0.1f, 0.1f, 0.5f, 0f);
    }
    private static void OnClose()
    {
        ImGui.SaveIniSettingsToDisk("imgui.ini");
        imgui.Dispose();
        input.Dispose();
        gl.Dispose();
    }
    
    private static void OnUpdate(double delta)
    {
        imgui.Update((float)delta);

        var imGuiViewport = ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(imGuiViewport.WorkPos);
        ImGui.SetNextWindowSize(imGuiViewport.WorkSize);
        ImGui.SetNextWindowViewport(imGuiViewport.ID);

        ImGuiWindowFlags dockSpaceFlags =
            ImGuiWindowFlags.NoDocking |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoNavFocus |
            ImGuiWindowFlags.NoSavedSettings;

        ImGui.Begin("Emulator", dockSpaceFlags);

        Update?.Invoke(delta);

        ImGui.End();

    }
    private static void OnRender(double delta)
    {
        gl.Clear(ClearBufferMask.ColorBufferBit);

        imgui.Render();
    }
}
