import re


def replace_content(content: str, map_content: str, unit_content: str):
    return content.replace("{{地图}}", map_content).replace("{{棋子}}", unit_content)


# 正则分析结果，把res 进行截取保存
def get_ins(text):
    pattern = r"---指令开始---(.*?)---指令结束---"
    matches = re.findall(pattern, text, re.DOTALL)
    if matches:
        return matches[0]
    else:
        return None
