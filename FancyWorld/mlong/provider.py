from abc import ABC, abstractmethod
import importlib
from pathlib import Path


class Provider(ABC):
    @abstractmethod
    def chat(self, model, messages):
        pass


class ProviderFactory:

    PROVIDER_DIR = Path(__file__).parent / "providers"

    @staticmethod
    def provider(provider, config):
        print(provider)
        print(config)
        provider_class_name = f"{provider.capitalize()}Provider"
        provider_module_name = f"{provider}_provider"

        module_path = f"mlong.providers.{provider_module_name}"
        print(module_path)

        try:
            module = importlib.import_module(module_path)
        except ImportError:
            raise ValueError(f"{provider} is not supported")

        provider_class = getattr(module, provider_class_name)
        return provider_class(**config)

    @classmethod
    def list_provider(cls):
        provider_files = Path(cls.PROVIDER_DIR).glob("*_provider.py")
        return {file.stem.replace("_provider", "") for file in provider_files}
