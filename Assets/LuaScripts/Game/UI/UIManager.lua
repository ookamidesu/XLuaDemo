
---@class UIManager
UIManager = BaseClass()

function UIManager:Constructor()
    
end

function UIManager:Init()
    print(123)
    self.canvas = {}
    self.uiRoot = GameObject.Find("UIRoot")
    local canvas = self.uiRoot.transform:Find("Canvas")
    self.hideNode = self.uiRoot.transform:Find("HideUI")
    self.canvas.Low = canvas:Find("Low")
    self.canvas.Normal = canvas:Find("Normal")
    self.canvas.Top = canvas:Find("Top")
    self.canvas.High = canvas:Find("High")
    print(self.canvas["High"].name)
end
