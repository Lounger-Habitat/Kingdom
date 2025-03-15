from typing import List, Dict, Any, Optional
from mlong.model_interface.utils import user, assistant, system

class ContextManager:
    """上下文管理器，负责管理对话上下文"""

    def __init__(self):
        """初始化上下文管理器"""
        self.context: List[Dict[str, Any]] = []

    def reset(self):
        """重置对话上下文"""
        self.context = []

    def clear(self):
        """清除对话上下文，但保留系统消息"""
        if len(self.context) != 0 and self.context[0]["role"] == "system":
            self.context = self.context[:1]

    @property
    def system(self) -> Optional[str]:
        """获取系统消息内容"""
        if len(self.context) != 0:
            if self.context[0]["role"] == "system":
                return self.context[0]["content"]
        return None

    @property
    def messages(self) -> List[Dict[str, Any]]:
        """获取当前对话上下文"""
        return self.context

    @system.setter
    def system(self, message: str):
        """设置系统消息"""
        if len(self.context) != 0:
            if self.context[0]["role"] == "system":
                self.context[0]["content"] = message
        else:
            self.context.append(system(message))

    def add_user_message(self, message: str):
        """添加用户消息
        
        Args:
            message: 用户消息内容
            
        Raises:
            ValueError: 如果用户消息不符合对话规则
        """
        # 检查是否有连续的用户消息
        if len(self.context) == 0:
            self.context.append(user(message))
        elif (
            self.context[-1]["role"] == "assistant"
            or self.context[-1]["role"] == "system"
        ):
            self.context.append(user(message))
        else:
            raise ValueError(
                f"User message must follow system or assistant message,now you context is {self.context},message is {message}"
            )

    def add_assistant_response(self, message: str):
        """添加助手响应
        
        Args:
            message: 助手响应内容
            
        Raises:
            ValueError: 如果助手响应不符合对话规则
        """
        if self.context and self.context[-1]["role"] == "user":
            self.context.append(assistant(message))
        else:
            raise ValueError("Assistant response must follow user message")

    def pop(self) -> Dict[str, Any]:
        """移除并返回最后一条消息"""
        return self.context.pop()


class ATAContextManager:
    """Agent to Agent 上下文管理器"""
    def __init__(self):
        """初始化上下文管理器"""
        self.context_manager = {
            "active": ContextManager(),
            "passive": ContextManager(),
            "topic": ContextManager(),
        }
    
    def reset(self):
        """重置上下文管理器"""
        for key in self.context_manager:
            self.context_manager[key].reset()
    def clear(self):
        """清除上下文管理器"""
        for key in self.context_manager:
            self.context_manager[key].clear()
    def get_context(self, key: str) -> List[Dict[str, Any]]:
        """获取指定角色的上下文
        Args:
            key: 角色名称
        Returns:
            上下文列表
        """
        return self.context_manager[key].messages
    def get_system(self, key: str) -> Optional[str]:
        """获取指定角色的系统消息
        Args:
            key: 角色名称
        Returns:
            系统消息内容
        """ 
        return self.context_manager[key].system

    @property
    def active(self) -> ContextManager:
        """获取主动角色的上下文管理器"""
        return self.context_manager["active"]
    @property
    def passive(self) -> ContextManager:
        """获取被动角色的上下文管理器"""
        return self.context_manager["passive"]
    @property
    def topic(self) -> ContextManager:
        """获取话题的上下文管理器"""
        return self.context_manager["topic"]

    @active.setter
    def active(self, context: ContextManager):
        """设置主动角色的系统消息"""
        self.context_manager["active"] = context
    @passive.setter
    def passive(self, context: ContextManager):
        """设置被动角色的系统消息"""
        self.context_manager["passive"] = context
    @topic.setter
    def topic(self, context: ContextManager):
        """设置话题的系统消息"""
        self.context_manager["topic"] = context