build_user_message = lambda text: {
    "role": "user",
    "content": [{"type": "text", "text": text}],
}
build_assistant_message = lambda text: {
    "role": "assistant",
    "content": [{"type": "text", "text": text}],
}
build_system_message = lambda text: {
    "role": "system",
    "content": [{"type": "text", "text": text}],
}

build_a_user_message = lambda text: {
    "role": "user",
    "content": text,
}
build_a_assistant_message = lambda text: {
    "role": "assistant",
    "content": text,
}
build_a_system_message = lambda text: {
    "role": "system",
    "content": text,
}
