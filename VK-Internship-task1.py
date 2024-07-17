import os
import requests
import winreg
import gdown


def download_file(url, directory):
  try:
    if not os.path.exists(directory):
      print(f'Директория {directory} не существует. Файл не будет сохранен.')
    else:
      file_id = url.split('id=')[-1]
      file_name = 'settings.reg'
      gdown.download(id=file_id, output=os.path.join(directory, file_name))

      print(f'Файл {file_name} успешно загружен в директорию {directory}.')

  except requests.exceptions.RequestException as e:
    print(f'Ошибка загрузки файла {file_name}: {e}')


def import_settings(settings_path):
  try:
    settings_data = {}

    with open(settings_path, 'r', encoding='utf-16') as reg_file:
      for line in reg_file:
        if line.startswith('['):
          registry_key = line[1:-2]
          index = registry_key.find('\\')
          registry_key = registry_key[index + 1:]
        elif line.startswith('"'):
          key_value = line.strip().split('=dword:')
          key = key_value[0].strip('"')
          value = int(key_value[1], 16)
          settings_data[key] = value

    key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, fr'{registry_key}')

    for data_key, data_value in settings_data.items():
      winreg.SetValueEx(key, data_key, 0, winreg.REG_DWORD, data_value)

    winreg.CloseKey(key)

    print(f'Значения файла {settings_path} успешно добавлены в реестр.')

  except FileNotFoundError:
    print(f'Файл {settings_path} не найден.')
  except Exception as e:
    print(f'Ошибка при импорте файла: {e}')


def run_game(game_path):
  try:
    os.startfile(game_path)
    print(f"Игра расположенная по пути {game_path} запущена")

  except FileNotFoundError:
    print(f"Игра не найдена по пути: {game_path}")
  except Exception as e:
    print(f"Ошибка при запуске игры: {e}")


if __name__ == "__main__":
  file_url = "https://drive.google.com/uc?export=download&id=1IGENwFzLm8bBEboISadYSNEdxbnjz1fH"
  path_to_directory = "C:\Program Files (x86)\Steam\steamapps\common\Goose Goose Duck"
  settings_path = f"{path_to_directory}\settings.reg"
  game_path = "C:\Program Files (x86)\Steam\steamapps\common\Goose Goose Duck\GGDLauncher.exe"

  download_file(file_url, path_to_directory)
  import_settings(settings_path)
  run_game(game_path)