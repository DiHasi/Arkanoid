   M S B A M L   `   `   `  ����  Arkanoid    Arkanoid.GameWindow   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	     � Title$  Arkanoid��5   	   $	��750��6   $	��800��6'    � 
ResizeMode$ NoResize=�63   .��5	      � .+�5
   
   �� -   5        $��Canvas��5
      
�������.+�5      �� -   5        $��TopWall��5   #   
 � Tag$
 base��6   
���iii�$��10��6A   
 �� Top$ 0��5      ��5      �� *ActualWidth+ �� ElementName$ Canvas��5      �� 5      -   5        $��
BottomWall��5   #   $
 base��6   
���iii�$��10��6D    �� Bottom$	 -10��5      ��5      �� *ActualWidth+$ Canvas��5      �� 5      -   5        $��LeftWall��5   #   $
 base��6   $��10��63    �� Left$ 0��5      
���iii���5   >   �� *ActualHeight+$ Canvas��5      �� 5      -   5        $��	RightWall��5   #   $
 base��6   $��10��64    �� Right$ 0��5      
���iii���5   ?   �� *ActualHeight+$ Canvas��5      �� 5      -   5        $��Platform��5   #   $
 base��6   $	��100��63   $��10��6?   
���   �$	 350��6X   $ 5��5      A� 5      -   5        $
��Ball��5      $��20��6"   $��20��6-   
���  ��$	 430��6D   $	 500��6V   5      5      5      