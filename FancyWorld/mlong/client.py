from typing import List
from mlong.provider import ProviderFactory
from mlong.types.type_chat import ChatResponse


class Client:
    def __init__(self, configs={}):

        # Configs
        self.configs = configs
        self.backends = {}

        # API
        self._chat = None
        self._embed = None

        # Init
        self.init_backends()

    def init_backends(self):
        for provider, config in self.configs.items():
            provider = self.validate(provider)
            self.backends[provider] = ProviderFactory.provider(provider, config)

    def validate(self, provider):
        available_provider = ProviderFactory.list_provider()
        if provider not in available_provider:
            raise ValueError(f"Provider {provider} is not supported")
        return provider

    def chat(self, model: str = "", messages: List[str] = [], **kwargs):

        print("model", model)
        print("messages", messages)
        if "/" not in model:
            raise ValueError("Model must be in the format of 'provider/model'")

        provider, model = model.split("/", 1)

        available_provider = ProviderFactory.list_provider()
        print(available_provider)

        if provider not in available_provider:
            raise ValueError(f"Provider {provider} is not supported")

        if provider not in self.backends:
            config = self.configs.get(provider, {})
            print("config", config)
            self.backends[provider] = ProviderFactory.provider(provider, config)

        model_client = self.backends.get(provider)

        if not model_client:
            raise ValueError(f"Provider {provider} is not supported")

        return model_client.chat(messages=messages, model=model, **kwargs)

    # @property
    # def embed(self):
    #     if not self._embed:
    #         self._embed = Embed(self)
    #     return self._embed
