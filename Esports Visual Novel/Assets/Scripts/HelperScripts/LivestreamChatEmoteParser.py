import csv;
import os;

ChatFileRelativePath = "\..\..\Resources\LivestreamMessages.csv"
Emotes = ["LUL", "OMEGALUL", "4Head", "ABOBA", "POG", "POGGERS", "widepepOMEGAKEKHappyChampHands", "PeepoHands", "PoggieWoggie"]

if __name__ == "__main__":
    print("\nStarting script...")
    dirname = os.path.dirname(__file__)
    dirname = dirname.replace("Scripts\HelperScripts", "StreamingAssets")
    write_dirname = dirname.replace("Messages", "Messages_TMP")
    headers = []
    content = []
    
    for filename in os.listdir(dirname):
        if filename.startswith("MinMax_") and filename.endswith(".csv") and filename != "MinMax_Usernames.csv":
            minmax_csv = dirname + "\\" + filename
            with open(minmax_csv, newline='', encoding='utf8') as chatFile:
                reader = csv.reader(chatFile, delimiter=",", quotechar='"')
                headers = next(reader)
                for row in reader:
                    new_message = ""
                    # Parse into words, then replace emote names with TMPro image macro
                    for word in row[0].split(" "):
                        if word in Emotes:
                            new_message += '<sprite="' + word + '" name="' + word + '"> '
                        else:
                            new_message += word + " "

                    # -1 to trim the last space
                    content.append([new_message[:len(new_message)-1]])

            # Write the file with replaced text
            with open(minmax_csv, 'w', encoding='utf8', newline='') as write_csv:
                writer = csv.writer(write_csv)

                writer.writerow(headers)
                writer.writerows(content)

            print("Replaced emotes in", minmax_csv)

    input("Press enter to quit.")
