#include <RmlUi/Core.h>
#include <algorithm>
#include <cstring>
#include <memory>

#define API extern "C" __declspec(dllexport)

// Only POD values, UTF-8 strings and borrowed buffers cross this boundary.
struct Callbacks
{
    uintptr_t (*compile)(const Rml::Vertex*, int, const int*, int);
    void (*draw)(uintptr_t, float, float, uintptr_t);
    void (*release_geometry)(uintptr_t);
    uintptr_t (*texture)(const unsigned char*, int, int);
    void (*release_texture)(uintptr_t);
    void (*scissor)(int, int, int, int);
    void (*transform)(const float*);
    double (*time)();
    void (*log)(int, const char*);
    void (*action)(const char*);
    int (*clipboard_read)(char*, int);
    void (*clipboard_write)(const char*);
};

static_assert(sizeof(Rml::Vertex) == 20);

class Renderer final : public Rml::RenderInterface
{
    Callbacks callbacks;
    Rml::Rectanglei clip;
    bool clipped = false;

public:
    explicit Renderer(Callbacks value) : callbacks(value)
    {
    }

    Rml::CompiledGeometryHandle
    CompileGeometry(Rml::Span<const Rml::Vertex> vertices, Rml::Span<const int> indices) override
    {
        return callbacks.compile(vertices.data(), int(vertices.size()), indices.data(), int(indices.size()));
    }

    void RenderGeometry(Rml::CompiledGeometryHandle geometry, Rml::Vector2f translation,
                        Rml::TextureHandle texture) override
    {
        callbacks.draw(geometry, translation.x, translation.y, texture);
    }

    void ReleaseGeometry(Rml::CompiledGeometryHandle geometry) override { callbacks.release_geometry(geometry); }

    Rml::TextureHandle LoadTexture(Rml::Vector2i&, const Rml::String& source) override
    {
        callbacks.log(1, ("External UI images are not enabled: " + source).c_str());
        return 0;
    }

    Rml::TextureHandle GenerateTexture(Rml::Span<const Rml::byte> pixels, Rml::Vector2i size) override
    {
        return callbacks.texture(pixels.data(), size.x, size.y);
    }

    void ReleaseTexture(Rml::TextureHandle texture) override { callbacks.release_texture(texture); }

    void EnableScissorRegion(bool enabled) override
    {
        clipped = enabled;
        SetScissorRegion(clip);
    }

    void SetScissorRegion(Rml::Rectanglei region) override
    {
        clip = region;
        if (clipped)
            callbacks.scissor(clip.Left(), clip.Top(), clip.Width(), clip.Height());
        else
            callbacks.scissor(0, 0, -1, -1);
    }

    void SetTransform(const Rml::Matrix4f* matrix) override { callbacks.transform(matrix ? matrix->data() : nullptr); }
};

class System final : public Rml::SystemInterface
{
    Callbacks callbacks;

public:
    explicit System(Callbacks value) : callbacks(value)
    {
    }

    double GetElapsedTime() override { return callbacks.time(); }

    bool LogMessage(Rml::Log::Type type, const Rml::String& text) override
    {
        callbacks.log(int(type), text.c_str());
        return true;
    }

    void GetClipboardText(Rml::String& text) override
    {
        const int length = callbacks.clipboard_read(nullptr, 0);
        if (length < 0)
            return;
        text.resize(size_t(length) + 1);
        callbacks.clipboard_read(text.data(), length + 1);
        text.resize(size_t(length));
    }

    void SetClipboardText(const Rml::String& text) override
    {
        callbacks.clipboard_write(text.c_str());
    }
};

class Actions final : public Rml::EventListener
{
    Callbacks callbacks;

public:
    explicit Actions(Callbacks value) : callbacks(value)
    {
    }

    void ProcessEvent(Rml::Event& event) override
    {
        for (auto* element = event.GetTargetElement(); element; element = element->GetParentNode())
        {
            const auto action = element->GetAttribute<Rml::String>("data-action", "");
            if (!action.empty())
            {
                callbacks.action(action.c_str());
                return;
            }
        }
    }
};

struct Ui
{
    Renderer renderer;
    System system;
    Actions actions;
    Rml::Context* context = nullptr;
    Rml::ElementDocument* document = nullptr;

    explicit Ui(Callbacks callbacks) : renderer(callbacks), system(callbacks), actions(callbacks)
    {
    }
};

static Ui* active_ui = nullptr;

API Ui* nd_rml_create(Callbacks callbacks, const char* font, const char* document)
{
    if (active_ui)
        return nullptr;
    auto ui = std::make_unique<Ui>(callbacks);
    Rml::SetSystemInterface(&ui->system);
    Rml::SetRenderInterface(&ui->renderer);
    if (!Rml::Initialise())
        return nullptr;
    if (Rml::LoadFontFace(font))
    {
        ui->context = Rml::CreateContext("NovoDwarf", {1280, 800});
        if (ui->context)
            ui->document = ui->context->LoadDocument(document);
    }
    if (!ui->document)
    {
        Rml::Shutdown();
        return nullptr;
    }
    ui->document->AddEventListener("click", &ui->actions);
    ui->document->Show(Rml::ModalFlag::None, Rml::FocusFlag::None);
    active_ui = ui.release();
    return active_ui;
}

API void nd_rml_destroy(Ui* ui)
{
    if (!ui)
        return;
    ui->document->RemoveEventListener("click", &ui->actions);
    Rml::Shutdown();
    delete ui;
    active_ui = nullptr;
}

API void nd_rml_update(Ui* ui, int width, int height, float scale)
{
    ui->context->SetDimensions({width, height});
    ui->context->SetDensityIndependentPixelRatio(scale);
    ui->context->Update();
}

API void nd_rml_render(Ui* ui) { ui->context->Render(); }
API int nd_rml_mouse_move(Ui* ui, int x, int y, int modifiers)
{
    return ui->context->ProcessMouseMove(x, y, modifiers);
}

API int nd_rml_mouse_button(Ui* ui, int button, int down, int modifiers)
{
    return down
               ? ui->context->ProcessMouseButtonDown(button, modifiers)
               : ui->context->ProcessMouseButtonUp(button, modifiers);
}

API void nd_rml_mouse_leave(Ui* ui) { ui->context->ProcessMouseLeave(); }
API void nd_rml_blur(Ui* ui) { ui->document->Focus(); }
API void nd_rml_wheel(Ui* ui, float x, float y, int modifiers) { ui->context->ProcessMouseWheel({x, y}, modifiers); }
API void nd_rml_key(Ui* ui, int key, int down, int modifiers)
{
    if (down)
        ui->context->ProcessKeyDown(Rml::Input::KeyIdentifier(key), modifiers);
    else
        ui->context->ProcessKeyUp(Rml::Input::KeyIdentifier(key), modifiers);
}

API void nd_rml_text(Ui* ui, const char* text) { ui->context->ProcessTextInput(Rml::String(text)); }
API int nd_rml_mouse_captured(Ui* ui) { return ui->context->IsMouseInteracting(); }
API int nd_rml_keyboard_captured(Ui* ui)
{
    auto* focused = ui->context->GetFocusElement();
    return focused && focused != ui->document && focused != ui->context->GetRootElement();
}

API void nd_rml_set_rml(Ui* ui, const char* id, const char* rml)
{
    if (auto* element = ui->document->GetElementById(id))
        element->SetInnerRML(rml);
}

API void nd_rml_set_property(Ui* ui, const char* id, const char* name, const char* value)
{
    if (auto* element = ui->document->GetElementById(id))
        element->SetProperty(name, value);
}

API void nd_rml_set_attribute(Ui* ui, const char* id, const char* name, const char* value)
{
    if (auto* element = ui->document->GetElementById(id))
    {
        if (value)
            element->SetAttribute(name, Rml::String(value));
        else
            element->RemoveAttribute(name);
    }
}

API int nd_rml_get_value(Ui* ui, const char* id, char* buffer, int capacity)
{
    auto* element = ui->document->GetElementById(id);
    auto* control = dynamic_cast<Rml::ElementFormControl*>(element);
    const auto value = control ? control->GetValue() : Rml::String();
    const int length = int(value.size());
    if (capacity > length)
        std::memcpy(buffer, value.c_str(), length + 1);
    return length;
}

API int nd_rml_checked(Ui* ui, const char* id)
{
    auto* element = ui->document->GetElementById(id);
    return element && element->HasAttribute("checked");
}

API int nd_rml_click(Ui* ui, const char* id)
{
    if (auto* element = ui->document->GetElementById(id))
    {
        element->Click();
        return 1;
    }
    return 0;
}
