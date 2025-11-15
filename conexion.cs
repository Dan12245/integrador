using Consumo_Reducido_de_Agua_ahora_si_definitivo;
using Npgsql;
using NpgsqlTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using Windows.System;
using static SkiaSharp.HarfBuzz.SKShaper;
using static Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View.Registro;
using Consumo_Reducido_de_Agua_ahora_si_definitivo.MVVM.View;

//ignoren este comentario solo es para llegar a las 600 lineas
//_. . ..._ . ._. __. ___ _. _. ._ __. .. ..._ . _.__ ___ .._ .._ .__. 
//vuelvan a ignorar esto, ya me valen las 600 lineas
/*
 * ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀ ⠀⢀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣿⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⣿⡿⣏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣸⡿⣟⣧⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣠⢤⣤⢶⡶⣶⢿⡿⣽⣳⢯⣞⡽⣿⢷⣶⣤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣤⢶⢺⣭⡶⣿⢾⡽⣯⢿⡽⣯⣟⡷⣯⣟⡾⣽⢯⣟⡾⣽⣻⣽⢻⡶⣶⢤⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⠞⣯⣵⡾⣟⡾⣽⢯⡿⣽⢯⣟⣽⣳⢯⡿⣵⢯⣻⣽⢻⡾⣝⡷⣽⣺⢯⡽⣞⣯⡷⣿⣗⡦⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣴⣮⣳⢿⡽⢾⡽⣏⡿⣽⠾⣽⣛⡾⣽⢾⣽⣫⢷⣯⢟⣧⣟⣯⣽⣛⣾⡳⣽⣫⠿⣽⢶⣻⣗⣯⢻⢯⣽⢶⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣯⣳⢯⣟⣳⣟⣯⡽⢯⣽⣳⣟⡷⣯⢿⣽⣫⢶⣯⢟⣞⣻⣞⠾⣵⢯⣟⣶⣻⣳⡽⢯⡷⣻⣖⡻⣎⢯⣟⡼⣻⡽⣷⣤⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣼⢳⡾⣝⣳⢯⣷⢻⡾⣽⣻⡞⣧⣟⡾⣽⣳⢾⣝⣯⣞⢿⣺⠷⣞⣿⣣⣟⣮⢷⣳⢯⡽⣯⢷⣻⣼⢳⡽⣺⢼⡹⢧⣻⢽⣯⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣀⣀⣤⠾⣬⡿⣝⡾⣭⣟⢾⢯⣟⡷⢯⣽⣳⢯⣟⡷⣫⣟⡾⣵⣞⡯⣷⢻⡽⣶⢻⣮⣟⡾⣽⠾⣽⣳⢯⢷⣫⣟⡾⢧⡻⣜⢯⡝⢯⣞⢷⣿⢤⣤⣤⡤⣤⢤⣄⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠖⣒⡶⣞⣷⡾⣿⣿⡿⣟⣿⣿⢚⣳⡿⣝⡧⣟⡶⢯⣟⣻⠾⣽⣻⠾⣝⣿⣺⣽⣳⢯⡷⣏⣾⢳⣏⣯⢷⣏⡿⣞⣼⡳⢯⣻⠵⣯⣯⣛⡷⣽⣫⢿⡱⣏⡞⡼⣣⢏⣞⡽⣯⣷⢯⡽⣮⣛⣴⣙⢆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡞⢮⢵⣻⡽⣾⣽⢿⣳⡿⣿⣻⣏⠧⣿⡽⣞⡽⣞⣽⣻⣾⣽⢻⣳⢯⢿⡽⣶⢻⡶⣯⣟⣳⣟⣞⣯⢾⣹⢾⣹⠾⣝⡾⣽⢯⣷⣻⢧⡟⣮⢽⣳⠿⡾⣝⢮⡵⢻⣱⣋⢮⢿⣳⣟⣯⢿⡵⣫⢶⣫⢯⣳⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡞⣝⢮⣳⢯⣟⡷⣯⢿⣽⣻⢷⡟⣬⢻⢷⣻⡼⢓⣭⠔⠆⠴⢮⡙⠿⣞⣯⠷⣯⣟⣽⣳⢯⢷⣯⣞⡾⣭⢷⣫⢯⢿⣹⢯⣽⣻⢾⡽⣳⢟⠋⠉⢁⡭⡙⠯⣞⡽⣣⠶⡹⢎⡻⣶⣹⣾⢿⡼⢧⣳⠷⣞⣺⡹⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⢿⢎⡷⢯⡿⣾⣟⡿⣿⢷⡿⣿⣛⡴⢯⣏⣷⢡⣿⢣⢚⠙⠂⠀⠀⢧⢹⣞⡿⣱⡞⣧⣟⣯⣿⣷⡻⢾⣱⢯⡞⣽⡺⣭⢿⣿⢿⣟⡞⣡⣊⠁⡀⠖⢢⠹⣷⠌⣷⢣⣟⡹⢫⡵⢫⢷⣿⢯⣟⢷⣫⣟⣼⣳⠿⣜⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⢯⣾⣽⣯⣽⣷⣯⣿⡽⣯⣟⣿⢶⣹⢳⢾⢧⣗⣻⣱⢊⡐⡼⢁⢀⡏⣼⣻⣵⢯⡽⣺⣷⣿⠏⣿⡻⢧⡟⣮⡝⣧⢻⡼⣻⡷⠘⢿⣇⢳⡸⠀⠀⠌⡆⡝⣿⡜⡾⣏⡶⣹⢇⣯⡻⣾⣹⣿⣎⣷⣿⣾⣿⣿⡿⣟⣧⣄⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⣤⣶⢿⣻⣟⡿⣽⢯⣿⣿⣿⣿⣿⡿⣵⣛⡞⣾⠬⣏⠳⠢⠃⠀⣈⠞⢠⣿⡳⢯⣞⣷⣿⡿⠃⠀⣟⣯⢷⡹⣖⠿⣜⣏⣳⣿⠅⠀⠈⢻⣮⢳⡀⠀⠈⠴⣹⢳⡟⣧⢻⣼⠳⣞⡲⣝⢧⣿⣿⣿⣿⣿⣿⢿⣯⣿⣽⣎⡷⣇⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⣼⢣⣟⣯⢷⣯⢿⣽⢿⣻⣿⣿⣿⣿⡟⣵⣺⢽⢣⠛⣄⠛⡌⠓⠖⠋⠄⣾⢷⡻⣝⣾⣿⡟⠁⠀⠈⣟⡟⣮⢳⣭⢻⡼⡜⣶⠏⠀⠀⠀⠀⣹⢆⠱⠤⠶⠆⡓⢣⢏⡹⡎⣷⢻⣜⡳⣭⢻⣿⣿⣿⢿⣳⢯⣿⣲⢞⣦⢫⠽⣺⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⣟⢳⡾⣭⣟⣾⡿⣾⣿⣿⣿⣿⣿⡿⣜⣣⢧⡿⡣⢍⡐⠢⠐⠁⠀⠂⢨⣟⡯⣽⣽⣿⠛⠳⣤⠀⢸⣫⡝⣮⢳⣎⠷⣭⣳⡟⠀⣠⡴⠞⠛⠉⡆⠀⠀⣱⠆⣉⠒⡌⢶⣹⡼⣏⡾⣱⢯⣳⢿⣿⣿⣿⣿⣟⡾⣽⢯⣖⣯⢫⠽⡇⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢰⡍⢾⣱⣟⣾⣳⣿⣿⣿⣿⣿⣿⡿⣵⣪⣽⢺⡟⣝⠦⣁⠣⡉⠄⠂⠀⣾⣻⣼⣿⡟⣁⡀⠀⠈⠀⣼⡳⣝⣞⡳⣎⡻⣴⡿⠡⠀⠋⠁⠀⠀⠀⢱⠀⠀⡀⡔⡄⠣⢜⣳⢧⡗⣯⠷⣭⣳⢏⡯⢿⣿⣽⣿⢿⣿⣿⣿⣾⣳⣯⣳⡇⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⢸⢯⣷⣿⣾⡷⢟⣻⡽⣯⣟⣿⣿⣿⣾⣿⡏⡽⢎⠣⣍⡑⡲⠥⠥⠤⢠⣿⣿⣿⣿⣷⣿⣭⡳⢦⣠⣟⣳⢭⡞⣵⢫⣵⡟⠃⠀⣀⣤⢶⣿⣿⣶⣾⡝⠀⣰⣔⡤⠗⢛⠩⢇⢿⢹⣏⢷⣯⣞⣻⣧⣿⣾⣽⣻⣞⡿⣿⣿⣿⣿⡿⠃⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠈⠒⠛⠾⡿⣡⢏⡼⣛⢷⣻⣞⣿⣿⣿⣿⢹⡅⢣⠂⠄⠉⠓⠒⠒⠀⣼⣿⡟⡁⡀⠹⣿⣿⠿⣷⣿⣹⣎⢷⡹⣎⣿⠏⠀⠀⠀⢙⣾⣿⣿⣿⢛⠟⠯⠀⠀⠤⠆⠚⠁⡘⡬⢚⡇⣿⢞⣿⣿⣿⣿⣿⡷⣯⢷⡾⣝⢯⠻⣿⡟⠁⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⢳⣎⣵⣫⣯⣷⣻⣯⣿⣿⡏⣸⠘⠤⢈⠂⠀⠀⠀⠀⢰⣿⣿⣿⣤⠀⣴⣿⣿⣷⢰⣿⢏⣟⣷⡿⠛⠁⠀⠀⠀⠀⠀⢁⣿⣿⣧⡈⠀⣤⡆⠀⠀⠀⠀⠐⠠⡁⠳⣣⢹⣿⣿⡿⣿⢿⣿⣻⣽⣟⡾⣽⣞⡳⣍⣛⢦⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⣠⠔⠊⠉⠩⠒⠤⣈⡯⣽⣹⢯⣟⡷⣿⣿⣿⣿⢃⢥⠋⡔⠀⡀⠀⠀⠀⠀⣼⣽⣿⣿⣷⣴⣾⣿⣿⣿⠀⣾⠿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⣸⣿⣿⣿⣅⣸⣿⣇⠀⠀⠀⠀⠀⠐⢠⠃⡵⡸⣿⣿⣽⢯⣟⡶⣯⢷⣻⣿⣷⠿⠛⠋⠭⠍⠣⡀⠀⠀⠀⠀⠀
⠀⠀⠀⢠⠇⠀⠀⡈⢠⠀⡈⠤⣡⣤⠏⠉⠉⠹⠉⠉⠹⠿⡈⡄⢁⠠⢀⠀⠀⠀⠀⠀⠁⢸⣿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⣿⣿⣿⠀⠀⠀⠀⠀⠁⡄⠸⠠⠧⣿⡿⡼⠿⡼⠿⠿⢿⡉⡡⠀⡀⠠⢠⠀⠀⠀⢌⡄⠀⠀⠀⠀
⠀⠀⠀⡏⠠⢐⠠⡘⣄⢣⡘⠤⣉⠺⣄⣠⠐⡄⢂⠔⡏⣼⠑⠬⡁⠆⡀⠀⠀⠀⠀⠀⠀⠈⠹⣿⣿⣿⣿⣾⣿⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠹⣿⣿⣿⣿⣿⣿⣿⠄⠀⠀⠀⠀⠐⣀⣣⠱⣈⢧⢒⡒⢲⠤⢀⠄⢂⡿⢇⠢⢡⡑⢢⠡⠌⢠⠘⡼⡄⠀⠀⠀
⠀⠀⢸⠇⡠⢃⡜⡱⢌⠆⣌⠱⣀⠶⠒⠋⠓⡬⣝⣾⠇⣯⡙⡆⡑⠀⠂⠠⠀⠀⠀⠂⣦⠀⠀⠀⠙⠛⠛⠛⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠛⠿⠿⠿⠟⠁⠀⠀⠀⠀⠀⠀⠀⠤⣒⡖⣸⢘⣧⣳⣱⠾⠬⠯⣅⡙⢌⠣⡘⢥⢋⡜⢠⠂⠄⡇⠀⠀⠀
⠀⠀⡼⢠⡑⢮⣜⣱⣋⢾⡬⠟⢁⠠⠀⠀⠀⢨⡑⣾⠩⣶⣥⢧⣀⠉⣜⠲⠄⠀⠀⢰⣿⣗⡤⢄⡀⠀⠀⠀⠀⠀⠀⠀⢀⣤⡀⠀⠀⠀⠀⢀⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡆⠀⠀⠀⠠⡖⠘⠠⣉⣿⣱⠬⣯⡁⡄⠀⠀⠀⡀⢉⠺⣥⡝⢦⢫⡜⣣⠞⡀⢻⠀⠀⠀
⠀⠀⣇⡌⢳⣬⢳⣧⣿⠏⠐⡨⠄⢂⠀⠐⠀⣶⡹⣿⢦⠻⣌⠐⠀⠈⣚⠠⠄⠀⠀⠈⠙⣿⣿⣷⣬⣦⡀⠀⠀⠀⠀⠀⠀⠈⠹⡒⠲⠶⠚⡛⠁⠀⠀⠀⠀⠀⠀⠀⠀⢠⠿⠃⠀⠀⠀⡅⣼⠒⠋⠉⢩⡿⢠⣿⡵⠱⠀⠀⠠⠐⢂⠴⠈⢻⣽⣳⢎⠵⣨⠔⡰⡂⠀⠀
⠀⡰⠘⢎⢦⡩⣟⣾⠏⡔⢡⠂⠍⡀⠀⠀⠀⢼⣟⡿⣜⢷⡈⠳⢄⣙⣁⡀⠀⠀⠀⠀⠀⠘⠿⠛⢛⠛⠷⣄⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⠤⠤⠤⣶⡋⠀⠀⠀⠀⠀⠀⠒⠶⢀⡰⠋⢤⡓⣼⣿⢃⠀⠀⠀⠌⢂⠜⡐⡀⢻⣯⡿⡞⣅⢢⡑⣇⠀⠀
⢠⠇⢸⡹⢶⣧⣝⡿⡐⢸⠀⢎⠐⠀⠀⠀⠀⢸⣿⡇⣿⢈⠙⠢⠖⢂⡈⠀⠀⠀⠀⡄⠀⠀⠀⡄⠀⠀⠄⠘⢿⣿⣭⣶⣢⣔⣤⣰⣀⣆⡰⣐⣲⣒⣶⠏⠀⠀⠀⢀⡆⠀⠀⠀⡄⠀⠀⠀⠉⢋⠁⡀⠘⡩⠒⣼⣟⣆⠀⠀⠀⠐⡈⠒⢄⡁⢂⣿⡷⣱⢎⡛⢄⠘⡄⠀
⣼⣸⠈⣗⢎⡿⣽⠇⡇⢠⠉⠤⢈⠀⠀⠀⠀⣿⣿⣇⠻⡀⠌⡁⠄⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⣇⠀⠀⠀⠀⠈⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⠃⠀⠀⠀⠀⢰⠇⠀⠀⠀⡆⠀⠀⠀⠀⠂⡀⠄⠡⡐⢹⢺⣿⣿⠀⠀⠀⠀⠄⡉⢆⠰⡀⢸⣿⢯⡧⣙⠌⢀⢳⠀
⡇⢿⡆⢺⡽⣻⣿⠘⠇⣏⠜⡀⢂⠀⠀⠀⠀⣿⣿⢃⠳⡘⠠⠐⡀⠀⠀⠀⠀⠀⣕⠀⠀⠀⠀⢹⠀⠀⠀⠀⠀⠀⢿⣿⡽⣿⣿⣿⣿⣿⡿⣿⡿⠁⠀⠀⠀⠀⠀⣼⠀⠀⠀⠄⢱⡀⠀⠀⠀⠂⠀⠌⡐⠌⡼⢜⣿⣿⠀⠀⠀⠀⠐⡈⡱⢂⠡⡇⣿⣯⢷⣡⢊⡔⡈⡆
⢧⣎⠱⡌⢟⡿⠃⠉⣰⠏⠒⡀⢀⠀⠀⠀⠠⣿⣿⡌⡓⢌⠡⢀⠀⠀⠀⠀⠀⢸⠁⠀⠀⠀⠀⢸⡄⠀⠀⠀⠀⠀⠘⢿⣿⣿⣿⣿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⢣⡏⠀⠀⠀⠀⠎⡇⠀⠀⠀⠄⢈⠑⡀⢎⠱⣊⢿⣿⡁⠀⠀⠀⠀⢁⠾⡁⠆⢣⠺⣿⣯⠉⣼⠰⣡⡇
⠈⢳⣋⠴⢘⡇⢅⠔⡨⢈⠁⠀⠁⠀⠀⠀⢸⣿⣏⢧⠱⡈⠐⠠⠀⠀⠀⠀⠀⡇⠀⠀⠄⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⡈⢿⣿⣯⢽⣫⢿⣿⠃⠀⠀⠀⠀⠀⠀⡘⡇⠀⠀⠀⠀⠀⢳⠀⠀⠀⠀⠂⠀⠆⢌⠲⣡⢾⣿⡇⠀⠀⠀⠀⠆⢃⡙⣦⠁⣂⠞⡇⣸⢍⣲⠏⠀
⠀⠀⠹⣧⣠⢇⠟⢠⢂⠡⠈⠀⠀⠀⠀⠀⢸⣿⠎⡖⡱⢈⠡⠀⠀⠀⠀⠀⢸⡁⠀⠀⠀⠀⠀⢸⡇⠠⠀⠀⠀⠀⠀⠀⠀⢿⣿⣿⣿⣿⠃⠀⠀⠀⠀⠀⠀⠠⢜⡇⠀⠀⠀⠀⢈⠘⡇⠀⠀⠀⠀⢈⠰⢈⠲⡡⢞⣿⣧⠀⠀⠀⠀⠀⠉⠰⢺⡔⡜⡆⡇⣴⡾⠃⠀⠀
⠀⠀⠀⢸⢊⠎⡐⢂⠆⠀⠀⠀⠀⠀⠀⠀⣿⣏⠞⡬⠑⡌⠠⠁⠀⠀⠀⠀⣿⢠⠀⠀⠀⠀⠀⢸⡟⡤⠀⠀⠀⠀⠀⠀⠀⠘⣿⣻⣿⠇⠀⠀⠀⠀⠀⠀⠀⢸⢼⡇⠀⠀⠀⠀⠀⢊⣷⢀⠀⠀⠀⠀⠄⢣⠡⡝⣌⢻⣿⠀⠀⠀⠀⠀⠀⠀⠒⡇⣌⠱⡱⡙⠁⠀⠀⠀
⠀⠀⢠⡟⢸⠠⡑⠌⠀⠀⠀⠀⠀⠀⠀⢸⡿⣩⠚⣴⠃⡌⢁⠀⠀⠀⠀⠀⡿⢸⠀⠀⠀⠀⠀⠀⢿⠶⠁⠀⠀⠀⠀⠀⠀⠀⢻⣿⡿⠀⠀⠀⠀⠀⠀⠀⢀⢻⣾⠱⠀⠀⠀⠀⠀⠂⣿⠘⠀⠀⠀⠀⠨⠄⠣⡜⠬⣍⢿⣆⠀⠀⠀⠀⠀⠀⠁⠹⣌⠦⢡⢸⡀⠀⠀⠀
⠀⠀⢸⡗⢸⢀⠡⠌⠀⠀⠀⠀⠀⠀⠀⢠⠿⡰⣉⠖⡍⠰⠀⠀⠀⠀⠀⠐⡟⡇⠀⠀⠀⠀⠀⠀⢸⡍⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠀⠀⠀⠀⠀⠀⠀⠀⢈⢚⡇⠀⠀⠀⠀⠀⠀⠐⣿⠘⠀⠀⠀⠀⠐⡌⠱⢨⠱⡌⠞⡃⠀⠀⠀⠀⠀⠀⠀⠡⢀⠎⠘⢠⡇⠀⠀⠀
⠀⠀⢸⡃⢰⠀⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⣽⣔⡡⠎⡌⢡⠀⠀⠀⠀⠀⢨⡗⠀⠀⠀⠀⠀⠀⠀⠘⢷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠼⠃⠀⠀⠀⠀⢀⢠⠀⢸⡄⠀⠀⠀⠀⠀⡰⢉⢆⠳⢌⠓⡀⠀⠀⠀⠀⠀⠀⠀⠀⠄⢨⠀⢸⡇⠀⠀⠀
⠀⠀⠀⠇⡻⢄⠡⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⢪⡑⢣⠘⡀⠀⠀⠀⠀⠀⡜⠀⣎⠀⠀⠀⠀⠀⠀⠀⠈⠀⠀⠀⠀⠀⠀⠀⠀⠀⡀⢀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢎⣣⠀⢻⠀⠀⠀⠀⠀⠄⣉⠦⣙⠬⢒⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠂⢴⢸⠅⠀⠀⠀
⠀⠀⠀⠘⣿⣌⠆⡁⠀⠀⠀⠀⠀⠀⠀⢢⡞⢦⡙⠄⢃⠐⠀⠀⠀⠀⠀⠃⠨⣏⠐⠀⠀⠀⠀⠀⠀⠀⠀⢀⠀⠀⠀⠀⠀⡄⠰⠀⠀⢆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢁⣳⡅⠀⠀⠀⠀⠀⠀⢈⠰⡘⢤⠛⣆⠂⠀⠀⠀⠀⠀⠀⠀⠠⢁⡚⢼⡞⠀⠀⠀⠀
⠀⠀⠀⠀⠈⠻⢧⣔⢂⠀⠀⠀⠀⢤⢨⣑⡞⡦⡑⠎⠄⡈⠀⠀⠀⠀⠀⠀⠀⢆⡃⠀⠀⠀⠀⠀⠀⠀⠐⠠⠌⠂⠄⠀⣨⠔⣀⢂⠀⢌⠆⠀⠀⠄⡠⠐⠀⠀⠀⠀⠀⠀⠀⠀⢚⡡⠱⠂⠀⠀⠀⠀⠀⢀⠊⡔⢣⡙⡴⣋⠔⡀⠀⠀⠀⠀⠀⣄⡳⣬⠏⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠉⠛⠿⡤⢀⡭⠶⡛⣼⠳⣡⠝⡌⠐⠀⠀⠀⠀⠀⠀⠀⢀⠃⢆⡁⠀⠀⠀⠀⠀⠀⢈⠐⣌⠐⡠⠌⢑⣣⠇⡄⢁⠢⠴⠂⠈⠐⠠⢁⠂⠀⠀⠀⠀⠀⠀⠀⢎⡔⠀⠀⠀⠀⠀⠀⠀⠀⠘⣌⠣⡜⡵⢣⡞⣔⠂⡄⠀⠐⡲⠞⠋⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⢾⣣⠳⢤⠃⡙⠻⣤⣁⠀⠀⠀⠀⠀⠀⠀⠠⡉⠄⠄⠡⠀⡀⠀⠠⠀⡀⠆⢀⠂⠐⣠⠞⣷⡾⠙⡄⠹⠦⢀⠡⠈⠰⢀⠠⠀⠀⠀⡀⠀⠤⠘⣀⠊⠄⡀⠀⠀⠀⠀⠀⠀⠀⣼⣧⠽⢈⠣⡜⢌⢲⡰⠞⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠻⣅⡛⠒⢦⡄⠑⠻⠄⠀⠀⢀⠐⡈⠅⡐⠈⠠⠁⢀⠀⠌⢀⠐⠀⠌⠨⠤⡅⢄⣶⠟⠀⠀⠘⢷⡎⢁⠂⢡⠂⠄⠂⠄⠁⡀⢀⠀⠠⢁⠀⠌⢂⠄⣀⠀⠀⠀⠀⣠⡾⠋⢁⡴⠊⠷⣪⠜⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢮⡀⠌⠄⠠⠀⡌⢤⠁⢎⣐⠂⠌⠌⡐⠌⡠⢌⠂⡄⢂⣂⠬⠈⢆⡼⠋⠀⠀⠀⠀⠀⠀⠉⠻⣌⠂⠂⢌⠠⠐⠠⠐⣀⠂⠡⠄⡨⠐⢠⠂⢆⡰⢀⠂⡀⢀⠀⠞⡡⢠⡵⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠓⢬⣄⣓⣔⢂⠩⢁⠢⢌⠱⡈⠴⣈⢔⡠⢒⡴⠮⢃⣘⡶⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠳⣎⣀⠻⢥⡒⠢⡄⢪⠁⢆⠡⢊⠅⠎⠔⡨⠍⣒⠠⠂⢈⣠⠕⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠁⠒⠭⣭⣉⢡⡐⣬⠵⢬⣣⢭⠲⣉⠦⠐⠀⣠⠎⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢢⠀⠂⠭⣗⢨⢣⡹⣌⡱⠌⡌⠡⢉⡐⢒⣒⣓⠗⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠓⠢⠼⣹⣉⠯⡽⣁⠦⠚⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⠢⠤⣈⢧⡃⢗⠺⡔⠷⣌⡳⠦⠓⠋⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠉⠉⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠑⠚⠓⠓⠋⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
*/

namespace Consumo_Reducido_de_Agua_ahora_si_definitivo
{
    //esta parte es importane pq aca hacemos el string de conexion no lo borren o nos quedamos sin base de datos
    class conexion
    {
        //si quieren cambiar de base de datos aca ponen los parametros nuevos y se genera solo
        //pero si la cadena es diferente ps nomas acomodan la wea
        NpgsqlConnection conex = new NpgsqlConnection();
        static string server = "aws-1-us-east-2.pooler.supabase.com";
        static string puerto = "6543";
        static string bd = "postgres";
        static string usuario = "postgres.eyroumbxtugceuwckkyg";
        static string password = "elpiggadekarim";
        /*
         * Datos para acceder a la base de datos
         "User Id=postgres.eyroumbxtugceuwckkyg;Password=[YOUR-PASSWORD];Server=aws-1-us-east-2.pooler.supabase.com;Port=6543;Database=postgres"
        */

        //cadena de conexion
        string cadena_conexion = "User Id=" + usuario + ";" + "Password=" + password + ";" + "Server=" + server + ";" + "Port=" + puerto + ";" + "Database=" + bd;
        //Esta funcion es relleno, solo es para comprobar si hay conexion con la base de datos
        public NpgsqlConnection establecer_conexion()
        {
            try
            {
                conex.ConnectionString = cadena_conexion;
                conex.Open();
                // Removed MessageBox de prueba
                conex.Close();

            }
            catch (Exception e)
            {
                MessageBox.Show("no se pudo conectar a la base de datos, error:" + e.ToString());
            }
            return conex;
        }
        //Esta wea esta separada en regiones para que sea mas facil de encontrar una funcion en caso de
        //que se necesite cambiar o algo
        //hablando de aca van las funciones que tienen que ver con el usuario principal de la app
        //digamos el arrendador (o arrendatario, no me acuerdo cual es cual y me da hueva buscar)
        #region Usuario principal
        //Con esta funcion registramos a un usuario nuevo en la base de datos
        public async Task <bool> registrar_usuario(string nombre, string email, string password)
        {

            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                   await con.OpenAsync();

                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                    string cadena = "INSERT INTO cra.users (name, email, password) VALUES (@Name, @correo, @contra) RETURNING user_id;";

                   await using (var ejecutor = new NpgsqlCommand(cadena, con))
                    {
                        ejecutor.Parameters.AddWithValue("@Name", nombre);
                        ejecutor.Parameters.AddWithValue("@correo", email);
                        ejecutor.Parameters.AddWithValue("@contra", passwordHash);

                        object result = await ejecutor.ExecuteScalarAsync();
                        if (result != null)
                        {
                            GlobalData.userid = Convert.ToInt32(result);
                            MessageBox.Show("Usuario registrado!");
                            return true;
                        }
                        else
                        {                            
                            return false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex); return false;
            }
        }
        //esta funcion es para iniciar sesion 
        public async Task<bool> iniciar_sesion(string email, string contraseña)
        {
            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                   await con.OpenAsync();

                    string query = "SELECT user_id, name, password FROM cra.users WHERE email = @correo";
                    await using (var ejecutor = new NpgsqlCommand(query, con))
                    {
                        ejecutor.Parameters.AddWithValue("@correo", email);

                        await using (var reader = await ejecutor.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string nombre = reader.GetString(1);
                                string passwordHash = reader.GetString(2);

                                if (BCrypt.Net.BCrypt.Verify(contraseña, passwordHash))
                                {
                                    GlobalData.userid = userId;
                                    GlobalData.UserName = nombre;
                                    GlobalData.email = email;
                                    return true; // sin MessageBox
                                }
                                else
                                {
                                    MessageBox.Show("Contraseña incorrecta.");
                                    return false;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No existe un usuario con ese correo.");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:"+ex);
                return false;
            }
        }

        //funcion para eliminar a un usuario
        public async void Eliminar_usuario(string email)
        {
            //En teoria ya deberia de borrar usuarios, y por ende deberian jalar todas las funciones
            //pq dependian del email, entonces cuando quieran usar una funcion de las que cree solamente
            //agregan esto para que saque el email y puedan usarla sin pedos pq la mayoria depende del
            //email y si acaso un dato extra que ocupen como el consumo o esas weas, pero eliminar un
            //usuario no ocupa tanto. Bno la line que deben agregar es esta:
            //conexion con = new conexion(); esto es para poder usar las funciones, aca no hay pedo con copiar y pegar
            //string user_email = GlobalData.email; esta linea es la importante ya que jala el email para poder usarlo
            //con.Eliminar_usuario(user_email); y ya mandamos llamar a la funcion que hace la chamba de eliminar al usuario
            try
            {
               await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    await con.OpenAsync();
                    //aca nomas hacemos un DELETE ya que postgress puede borrar todo con una configuracion
                    //que ya esta activada para borrar todo lo relacionado con la foreign key, en este caso
                    //el id del usuario, asi que ya no hay que meter mas que este query
                    string query = "DELETE FROM cra.users WHERE email = @email;";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@email", email);
                        command.ExecuteNonQuery();
                    }
                    MessageBox.Show("usuario eliminado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }
        #endregion
        //aca ponemos funciones relacionadas a los domicilios
        #region domicilio
        //Funcion para agregar domicilio
        public async void agregar_domicilio_async(string alias, string descripcion, int userId)
        {
            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    await con.OpenAsync();                    
                    string query = "INSERT INTO cra.buildings (user_id, alias, description) VALUES (@user_id, @alias, @description)";

                    await using (var ejecutor = new NpgsqlCommand(query, con))
                    {
                        ejecutor.Parameters.AddWithValue("@user_id", userId);
                        ejecutor.Parameters.AddWithValue("@alias", alias);
                        ejecutor.Parameters.AddWithValue("@description", descripcion);
                        await ejecutor.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }

        //creo que el nombre explica bien lo que hace la funcion
        public async void eliminar_domicilio(string alias, int user_id)
        {
            try
            {
                // hacemos la conexion y la abrimos
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    //hacemos nuestro query para buscar alias del domicilio a eliminar               
                    await con.OpenAsync();
                    string query = "DELETE FROM cra.buildings WHERE alias=@alias and user_id=@user_id";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@alias", alias);
                        command.Parameters.AddWithValue("@user_id", user_id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }

        //con esto cambiamos los datos del domicilio que lit nomas es la descripcion y el alias
        //El que quiera cambiar eso nomas es pq le van a checar el fono yo creo
        public async Task<bool> editar_domicilio(string alias, int buildingId)
        {
            NpgsqlConnection.ClearAllPools();
            try
            {
                await using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    //hacemos nuestro query para buscar alias del domicilio a eliminar               
                    await con.OpenAsync();
                    //jalamos el id del usuario para poder buscar el edificio correcto
                    //hacemos nuestro query para buscar el id y lo almacenamos en nuestra variable
                    //estas variables se van a pasar por text box pero como no existen todavia se quedan en variables
                    string descripcion = "tiene una puerta, un cuarto y tiene 3 baños";
                    string query = "UPDATE cra.buildings SET alias=@alias, description=@description WHERE building_id=@buildingId";
                    await using (NpgsqlCommand command = new NpgsqlCommand(query, con))
                    {
                        //ejecutamos el query y guardamos el id
                        command.Parameters.AddWithValue("@alias", alias);
                        command.Parameters.AddWithValue("@description", descripcion);
                        command.Parameters.AddWithValue("@buildingId", buildingId);
                        object result = await command.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return false;
            }

        }
        #endregion
        //aca son funciones para sacar los ID de usuario, consumo, casa e invitado
        //borran estas weas y todo truena pq la mayoria de las funciones sacan datos de aca
        #region id
        public async Task<int> id_usuario(string email)
        {
            NpgsqlConnection.ClearAllPools();
            try
            {
                await using var cone = new NpgsqlConnection(cadena_conexion);
                await cone.OpenAsync();                
                string query = "SELECT user_id FROM cra.users WHERE email = @correo";
                await using var command = new NpgsqlCommand(query, cone);
                command.Parameters.AddWithValue("@correo", email);

                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        public async Task<int> id_edificio(int userId)
        {
            try
            {
                await using var con = new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();

                string query = "SELECT building_id FROM cra.buildings WHERE user_id = @user_id";
                await using var command = new NpgsqlCommand(query, con);
                command.Parameters.AddWithValue("@user_id", userId);

                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        public async Task<int> consumo_id(int building_id)
        {
            try
            {
                await using var con = new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();

                string query = "SELECT id FROM cra.consumption_per_day WHERE building_id = @building_id";
                await using var command = new NpgsqlCommand(query, con);
                command.Parameters.AddWithValue("@building_id", building_id);

                object result = await command.ExecuteScalarAsync();
                if (result == null || result == DBNull.Value)
                    return -1;

                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return -1;
            }
        }

        #endregion
        //aca van las funciones para los invitados
        #region invitados
        public async Task<bool> existe_invitacion(string email)
        {
            string invitacion = "437208959";
            using (var con = new NpgsqlConnection(cadena_conexion))
            {
                con.OpenAsync();
                int user_id = await id_usuario(email);
                string query = "SELECT COUNT(*) FROM cra.users WHERE inv_code=@inv_code";
                using (var ejecutor = new NpgsqlCommand(query, con))
                {
                    ejecutor.Parameters.AddWithValue("@inv_code", invitacion);
                    int existe = Convert.ToInt32(ejecutor.ExecuteScalar());
                    if (existe == 0)
                    {
                        MessageBox.Show("Código de invitación no válido.");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

        }
        //no se si esta funcion va aca o neh, asi que por mientras se queda aca
        public async Task<bool> Invitar_usuario(string email, string name, bool y_o_n)
        {
            try
            {
                //esta variable tambien se puede borrar sin tanto pedo, nomas ando esperando a que
                //suelten la version con los botones para lo demas
                string invitacion = "437208959";
                using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    con.Open();
                    int user_id = await id_usuario(email);
                    if (await existe_invitacion(email))
                    {
                        string query = "INSERT INTO cra.invited_users (user_id,name,read_only) VALUES (@user_id,@name,@read_only)";
                        using (var ejecutor = new NpgsqlCommand(query, con))
                        {
                            ejecutor.Parameters.AddWithValue("@user_id", user_id);
                            ejecutor.Parameters.AddWithValue("@name", name);
                            ejecutor.Parameters.AddWithValue("@read_only", y_o_n);
                            ejecutor.ExecuteNonQuery();

                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
                return false;
            }
        }
        public async Task editar_invitado(string email, string name, bool read_only)
        {
            try
            {
                using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    int user_id = await id_usuario(email);
                    if (await existe_invitacion(email))
                    {
                        con.Open();
                        string query = "UPDATE cra.invited_users SET user_id=@user_id and @read_only=read_only WHERE building_id=@building_id AND day=@day";
                        using (var ejecutor = new NpgsqlCommand(query, con))
                        {
                            ejecutor.Parameters.AddWithValue("@user_id", user_id);
                            ejecutor.Parameters.AddWithValue("@read_only", read_only);
                            ejecutor.ExecuteNonQuery();
                            MessageBox.Show("Consumo cambiado");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }
        }
        public async Task eliminar_invitado(string email, string name)
        {
            try
            {
                using (var con = new NpgsqlConnection(cadena_conexion))
                {
                    con.Open();
                    int user_id = await id_usuario(email);
                    if (await existe_invitacion(email))
                    {
                        string query = "DELETE FROM cra.invited_users WHERE user_id = @user_id and name=@name;";
                        using (var ejecutor = new NpgsqlCommand(query, con))
                        {
                            ejecutor.Parameters.AddWithValue("@user_id", user_id);
                            ejecutor.Parameters.AddWithValue("@name", name);
                            ejecutor.ExecuteNonQuery();
                        }
                        MessageBox.Show("invitado eliminado");
                    }
                    else
                    {
                        MessageBox.Show("invitado no encontrado");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex);
            }

        }
        #endregion
        #region Reportes
        public async void reportes(int user_id, string type, string description)
        {
            try
            {
                await using var con= new NpgsqlConnection(cadena_conexion);
                await con.OpenAsync();
                int id = Login.userid;
                string com = "INSERT INTO cra.bugs (user_id, type, description) VALUES (@user_id, @type, @description)";
                await using (var ejecutor = new NpgsqlCommand(com, con))
                {
                    ejecutor.Parameters.AddWithValue("@user_id", id);                    
                    ejecutor.Parameters.AddWithValue("@type", type);
                    ejecutor.Parameters.AddWithValue("@description", description);
                    await ejecutor.ExecuteNonQueryAsync();
                }
                MessageBox.Show("Error enviado");
            }
            catch (Exception ex) {
                MessageBox.Show("error, "+ex.ToString());
            }
        }
        #endregion
        //y las funciones relacionadas con el consumo

        #region metodos edicion de consumo
        public async Task<List<(int Id, string Alias)>> GetBuildingsForUser(int userId)
         {
             var result = new List<(int, string)>();
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 string sql = "SELECT building_id, COALESCE(alias,'Edificio '||building_id) FROM cra.buildings WHERE user_id=@uid ORDER BY building_id";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@uid", userId);
                 await using var reader = await cmd.ExecuteReaderAsync();
                 while (await reader.ReadAsync())
                 {
                     int bid = reader.GetInt32(0);
                     string alias = reader.GetString(1);
                     result.Add((bid, alias));
                 }
             }
             catch (Exception ex) { Console.WriteLine("GetBuildingsForUser error:" + ex); }
             return result;
         }

         public async Task<List<(DateTime Day, double Consumption)>> GetConsumptionForBuilding(int buildingId, int year)
         {
             var result = new List<(DateTime, double)>();
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 DateTime start = new DateTime(year,1,1);
                 DateTime end = new DateTime(year,12,31);
                 string sql = "SELECT day, consumption FROM cra.consumption_per_day WHERE building_id=@bid AND day BETWEEN @d1 AND @d2 ORDER BY day";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@bid", buildingId);
                 cmd.Parameters.Add("@d1", NpgsqlDbType.Date).Value = start.Date;
                 cmd.Parameters.Add("@d2", NpgsqlDbType.Date).Value = end.Date;
                 await using var reader = await cmd.ExecuteReaderAsync();
                 while (await reader.ReadAsync())
                 {
                     var day = reader.GetDateTime(0);
                     // consumption es float4 (real) -> leer como Single y convertir a double
                     float consF = reader.GetFloat(1);
                     result.Add((day, (double)consF));
                 }
             }
             catch (Exception ex) { Console.WriteLine("GetConsumptionForBuilding error:" + ex); }
             return result;
         }
         public async Task<bool> UpsertConsumption(int buildingId, DateTime day, double consumption)
         {
             try
             {
                 // asegurar índice único (userId + buildingId) antes de insertar
                 await EnsureConsumptionUniqueIndexAsync();
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 var dateVal = day.Date;
                 var realVal = (float)Math.Round(consumption,2, MidpointRounding.AwayFromZero);
                 string sql = "INSERT INTO cra.consumption_per_day(building_id, day, consumption) VALUES(@b,@d,@c) ON CONFLICT (building_id, day) DO UPDATE SET consumption=EXCLUDED.consumption";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@b", buildingId);
                 cmd.Parameters.Add("@d", NpgsqlDbType.Date).Value = dateVal;
                 cmd.Parameters.Add("@c", NpgsqlDbType.Real).Value = realVal;
                 await cmd.ExecuteNonQueryAsync();
                 return true;
             }
             catch (Exception ex)
             {
                 Console.WriteLine("UpsertConsumption error:" + ex);
                 return false;
             }
         }
         public async Task<bool> DeleteConsumption(int buildingId, DateTime day)
         {
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 string sql = "DELETE FROM cra.consumption_per_day WHERE building_id=@b AND day=@d";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 cmd.Parameters.AddWithValue("@b", buildingId);
                 cmd.Parameters.Add("@d", NpgsqlDbType.Date).Value = day.Date;
                 int affected = await cmd.ExecuteNonQueryAsync();
             return affected >0;
             }
                 catch (Exception ex) { Console.WriteLine("DeleteConsumption error:" + ex); return false; }
         }
         #endregion
         // Asegura el índice único requerido para ON CONFLICT (buildingID + UserID)

         public async Task EnsureConsumptionUniqueIndexAsync()
         {
             try
             {
                 await using var con = new NpgsqlConnection(cadena_conexion);
                 await con.OpenAsync();
                 const string sql = "CREATE UNIQUE INDEX IF NOT EXISTS ux_consumption_building_day ON cra.consumption_per_day (building_id, day)";
                 await using var cmd = new NpgsqlCommand(sql, con);
                 await cmd.ExecuteNonQueryAsync();
             }
             catch (Exception ex)
             {
                 Console.WriteLine("EnsureConsumptionUniqueIndexAsync error:" + ex);
             }
         }
    }
}
