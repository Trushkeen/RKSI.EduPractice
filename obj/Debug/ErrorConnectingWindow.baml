   M S B A M L   `   `   `  ����K  GRKSI.EduPractice, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:RKSI.EduPracticeRKSI.EduPractice  5   	   ,    &RKSI.EduPractice.ErrorConnectingWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:RKSI.EduPractice     5   	     � Title$  ErrorConnectingWindow��5   	   $��320.5��6'   $��307.5��66    � 
ResizeMode$ NoResize=�6D   .��5	      � .+�5
   
   �� $��Center=�6   $����Не удалось подключиться к базе данных!
Перенастройте параметры подключения:��64   $��Center=�6�   $��
19,10,24,0q�6�   $	��Top=�6�   $	��247��6�   �� 5   
   $��IP сервера:��6   $
��Left=�6&   $��	10,56,0,0q�6A   $	��Top=�6T   �� 5   
   -   5        $��
tbServerIp��5      $
��Left=�6$   $��23��6?   $��	10,82,0,0q�6K    �� TextWrapping$
 Wrap=�6^   $	��Top=�6r   $	��120��6�   �� 5   
   $��	Порт:��6   $
��Left=�6    $��
159,56,0,0q�6;   $	��Top=�6O   $��45��6g   �� 5   
   -   5        $��tbServerPort��5      $
��Left=�6&   $��23��6A   $��
159,82,0,0q�6M   $
 Wrap=�6a   $	��Top=�6u   $��79��6�   �� 5   
   -   5        $��btnInsertDefaultPort��5      $
��5432��6-   $
��Left=�6<   $��
243,82,0,0q�6W   $	��Top=�6k   $��36��6�   $��23��6�   6�   �� 5   
   $��Пользователь:��6   $
��Left=�6(   $��
10,110,0,0q�6C   $	��Top=�6W   $��93��6o   �� 5   
   -   5        $��tbServerUser��5      $
��Left=�6&   $��23��6A   $��
10,136,0,0q�6M   $
 Wrap=�6a   $	��Top=�6u   $	��120��6�   �� 5   
   $��Пароль:��6   $
��Left=�6"   $��159,110,0,0q�6=   $	��Top=�6R   �� 5   
   -   5        $��tbServerUserPassword��5      $
��Left=�6.   $��23��6I   $��159,136,0,0q�6U   $
 Wrap=�6j   $	��Top=�6~   $	��120��6�   �� 5   
   $5��/Используемая база данных:��6   $
��Left=�64   $��
10,164,0,0q�6O   $	��Top=�6c   �� 5   
   -   5        $��tbServerUsableDb��5      $
��Left=�6*   $��24��6E   $��
10,190,0,0q�6Q   $
 Wrap=�6e   $	��Top=�6y   $	��269��6�   �� 5   
   -   5        $��
btnConnect��5      $��Подключиться��6$   $
��Left=�6;   $��
10,219,0,0q�6V   $	��Top=�6j   $	��269��6�   6�   �� 5   
   -   5        $��btnUseWithoutConnection��5      $<��6Использовать без подключения��60   $
��Left=�6W   $��
10,244,0,0q�6r   $	��Top=�6�   $	��269��6�   6�   5      5      