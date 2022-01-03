---@class LoginView : BasePanel
local LoginView = require("Game/UI/Login/LoginViewGenerate")

function LoginView:OnLoad()
    self:InitEvent()
end

function LoginView:Init(controller)
    ---@type LoginController
    self.ctrl = controller
end

function LoginView:InitEvent()
    
    self.btnLogin.onClick:AddListener(function()
        self:ClickLoginButton()
    end)
end

function LoginView:ClickLoginButton()
    self.ctrl:StartLogin("123","123")
end

return LoginView
