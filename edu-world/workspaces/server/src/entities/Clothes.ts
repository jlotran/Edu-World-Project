import { Column, CreateDateColumn, Entity, PrimaryGeneratedColumn, UpdateDateColumn } from 'typeorm';

@Entity({ name: 'clothes' })
export class Clothes {
  @PrimaryGeneratedColumn('uuid')
  clothes_id!: string;

  @Column({type: 'boolean', nullable: false})
  gender!:number;

  @Column({type: 'varchar', length: 7, default: "#FFFFFF"})
  base_color!:string;

  @Column({type: 'varchar', length: 255, nullable: false})
  category!:string;
  
  @Column({ type: 'int', default: 1 })
  status!: number;

  @Column({ type: 'int' , default: 0 })
  price!: number;

  @Column({ type: 'varchar', length: 20 , nullable: false })
  rarity!: string;

  @CreateDateColumn()
  created_at!: Date;

  @UpdateDateColumn()
  updated_at!: Date;

}
