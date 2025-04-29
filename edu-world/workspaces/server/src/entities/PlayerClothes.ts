import { Column, Entity, ManyToOne, PrimaryGeneratedColumn, CreateDateColumn, JoinColumn } from 'typeorm';

import { Clothes } from './Clothes';
import { User } from './User';

@Entity({ name: 'player_clothes' })
export class PlayerClothes {
  @PrimaryGeneratedColumn('uuid')
  id!: string;

  @ManyToOne(() => User, (player) => player.id)
  @JoinColumn({ name: 'player_id' })
  player_id!: User;

  @ManyToOne(() => Clothes, (clothes) => clothes.clothes_id)
  @JoinColumn({ name: 'clothes_id' })
  clothes_id!: Clothes

  @Column({ type: 'boolean' , default: false})
  equipped!: boolean;

  @Column({ type: "varchar", length: 7, default: "#FFFFFF"})
  custom_color!: string

  @Column({type: 'boolean', default: false})
  is_deleted!: number

  @CreateDateColumn()
  created_at!: Date;

  @CreateDateColumn()
  updated_at!: Date;

}
