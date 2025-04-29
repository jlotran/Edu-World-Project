import { Column, Entity, ManyToOne, PrimaryGeneratedColumn, CreateDateColumn, UpdateDateColumn } from 'typeorm';

@Entity({ name: 'cars' })
export class Car {
  @PrimaryGeneratedColumn('uuid')
  car_id!: string;

  @Column({ type: 'varchar', length: 255 , nullable: false })
  model_name!: string;

  @Column({ type: 'varchar', length: 7, default: "#FFFFFF" , nullable: false })
  color!: string;

  @Column({ type: 'int' , nullable: false })
  speed!: number;

  @Column({ type: 'int' , nullable: false })
  handling!: number;

  @Column({ type: 'int' , nullable: false })
  nitro!: number;

  @Column({ type: 'int' , default: 0 })
  price!: number;

  @Column({ type: 'varchar', length: 20 , nullable: false })
  rarity!: string;

  @Column({ type: 'varchar', length: 255 , nullable: false })
  car_code!: string;

  @Column({ type: 'decimal', precision: 5, scale: 2, default: 0.0, nullable: false })
  acceleration!: number;

  @CreateDateColumn()
  created_at!: Date;

  @UpdateDateColumn()
  updated_at!: Date;
}
/**
 * car_id: int
  model_name: varchar(255)
  color: mã màu 
  speed: int 
  accelaration: demical 0.0  
  handling: int 
  Nitro: int 
  price: 
  rarity:
 */