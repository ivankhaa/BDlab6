   M S B A M L   `   `   `  ����@  <WpfBD, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:WpfBDWpfBD  5   	       WpfBD.ConnectionWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:WpfBD     5   	     � Title$  ConnectionWindow��5   	   $	��220��6"   $	��500��6/   $	��220��6<   $	��300��6L   .��5	      �  � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   �� 5      $��20 �6#    5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5      �� $��0��6   $��Server:��6!   �� 5      -   5        $��tbServer��5   4   $��0��6   $��1��6#   $��	localhost��6D   �� 5      $��1��6   $��User:��6!   �� 5      -   5        $��tbUser��5   3   $��1��6   $��1��6#   $
��root��6A   �� 5      $��2��6   $��	Database:��6!   �� 5      -   5        $��
tbDatabase��5   3   $��2��6   $��1��6#   $
��test��6E   �� 5       $��3��6   $��	Password:��6!   G� 5!      -   5        $��
tbPassword��5!   7   $��3��6   $��1��6'   67   5"      � 5#   
   $��1��6    5$      �� 5%      $��* �6#    5'      �� 5(      $��30 �6    �� 5)      $
��Auto �6    .+�5+      �� -   5        $��lblError��5+      $�� ��6$   
���  ��$y�	Collapsed=�6@   �� 5,      -   5        $��1��5,      $��1��6"   $��Center=�63   $	��100��6Q   $��Connect��6^   $��0,10,0,0q�6�   5-      5.      5/      